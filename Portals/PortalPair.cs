using RWCustom;

namespace Portals;

public class PortalPair
{
    public PortalPair(Player player)
    {
        this.player = player;

        a = new Portal(player, 0, new IntVector2(-1, -1));
        b = new Portal(player, 1, new IntVector2(-1, -1));
    }

    public void SetPortal(int id, Room room)
    {
        IntVector2? pos = GetPosForPortal(player, out IntVector2 dir);
        Portal portal = GetPortal(id);

        if (pos == null)
        {
            portal.RemoveFromRoom();
            return;
        }
        
        portal.RemoveFromRoom();
        room.AddObject(portal);
        portal.tilePos = pos.Value;
        portal.dir = dir;
        
        PortalPlugin.Log.LogInfo($"set portal for {player.playerState.playerNumber}:{id} at {pos.Value}");
    }
    
    private static IntVector2? GetPosForPortal(Player player, out IntVector2 dir)
    {
        IntVector2 pos = player.room.GetTilePosition(player.firstChunk.pos);
        dir = GetThrowDirection(player);

        while (player.room.IsPositionInsideBoundries(pos))
        {
            pos += dir;

            if (player.room.GetTile(pos).Solid)
            {
                return pos;
            }
        }

        return null;
    }

    private static IntVector2 GetThrowDirection(Player player)
    {
        IntVector2 dir = player.input[0].IntVec;
        if (dir.x != 0 || dir.y != 0)
            return dir;
        
        dir = new IntVector2(player.ThrowDirection, 0);
        if (player.animation == Player.AnimationIndex.Flip && player.input[0].y < 0 && player.input[0].x == 0)
        {
            dir = new IntVector2(0, -1);
        }

        return dir;
    }
    
    private Portal GetPortal(int id) => id == 0 ? a : b;


    public readonly Player player;
    private readonly Portal a;
    private readonly Portal b;

}
