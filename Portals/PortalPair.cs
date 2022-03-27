using RWCustom;

namespace Portals;

public class PortalPair
{
    public PortalPair(Player player)
    {
        this.player = player;
        _portals = new []
        {
            new Portal(0, player.room, new IntVector2(-1, -1)),
            new Portal(1, player.room, new IntVector2(-1, -1))
        };
    }

    public void SetPortal(int id, Room room)
    {
        if (!TryShootPortal(player, out IntVector2 tilePos, out IntVector2 dir))
            return;
        
        Portal portal = _portals[id];
        
        portal.RemoveFromRoom();
        room.AddObject(portal);
        portal.SetPos(tilePos, dir);

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
    private readonly Portal[] _portals;

    public Portal A => _portals[0];
    public Portal B => _portals[1];
}
