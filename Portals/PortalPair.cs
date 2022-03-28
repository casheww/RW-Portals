using RWCustom;
using UnityEngine;

namespace Portals;

public class PortalPair
{
    public PortalPair(Player player)
    {
        this.player = player;
        playerLastRoom = player.room;
        _portals = new []
        {
            new Portal(0),
            new Portal(1)
        };
    }

    public void Update()
    {
        if (player.room == null) return;
        
        if (player.room != playerLastRoom)
        {
            A.ClearFromRoom();
            B.ClearFromRoom();
            playerLastRoom = player.room;
        }
        
        if (Input.GetKeyDown(KeyCode.D))
            SetPortal(0);
        else if (Input.GetKeyDown(KeyCode.F))
            SetPortal(1);
    }
    
    public void SetPortal(int id)
    {
        if (!TryShootPortal(player, out IntVector2 tilePos, out IntVector2 dir))
            return;
        
        _portals[id].SetPos(player.room, tilePos, dir);

        PortalPlugin.Log.LogInfo($"set portal for {player.playerState.playerNumber}:{id} at {tilePos}");
    }
    
    private static bool TryShootPortal(Player player, out IntVector2 pos, out IntVector2 dirIntoPortal)
    {
        pos = player.room.GetTilePosition(player.firstChunk.pos);
        IntVector2 dir = GetThrowDirection(player);
        //IntVector2 dirPerp = Custom.PerpIntVec(dir);

        while (player.room.IsPositionInsideBoundries(pos))
        {
            pos += dir;

            if (player.room.GetTile(pos).Solid)
            {
                dirIntoPortal = dir;
                return true;
            }

            /*
            player.room.IdentifySlope(pos) == 

            if (player.room.GetTile(pos).Solid &&
                player.room.GetTile(pos + dirPerp).Solid &&
                player.room.GetTile(pos - dirPerp).Solid &&
                !player.room.GetTile(pos + dirPerp - dir).Solid &&
                !player.room.GetTile(pos - dirPerp - dir).Solid)
            {
                dirIntoPortal = dir;
            }
            else*/
        }

        dirIntoPortal = default;
        return false;
    }

    private static IntVector2 GetThrowDirection(Player player)
    {
        IntVector2 throwDir = player.input[0].IntVec;
        if (throwDir.x != 0 || throwDir.y != 0)
            return throwDir;
        
        throwDir = new IntVector2(player.ThrowDirection, 0);
        if (player.animation == Player.AnimationIndex.Flip && player.input[0].y < 0 && player.input[0].x == 0)
        {
            throwDir = new IntVector2(0, -1);
        }

        return throwDir;
    }

    public readonly Player player;
    private Room playerLastRoom;
    private readonly Portal[] _portals;

    public Portal A => _portals[0];
    public Portal B => _portals[1];
}
