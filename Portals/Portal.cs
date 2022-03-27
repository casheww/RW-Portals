using RWCustom;
using UnityEngine;

namespace Portals;

public class Portal : CosmeticSprite
{
    public Portal(int id, Room room, IntVector2 tilePos)
    {
        this.room = room;
        portalID = id;
        _tilePos = tilePos;
    }
    
    public override void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
    {
        sLeaser.sprites = new []
        {
            new FSprite("Circle20")
        };
        
        AddToContainer(sLeaser, rCam, null);
        base.InitiateSprites(sLeaser, rCam);
    }

    public override void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
    {
        FSprite sprite = sLeaser.sprites[0];
        sprite.color = portalID == 0 ? new Color(0.7f, 0.2f, 0.2f) : new Color(0.2f, 0.6f, 0.2f);
        
        sprite.scaleX = 0.5f;
        sprite.scaleY = 3f;
        sprite.SetPosition(rCam.room.MiddleOfTile(_tilePos.x, _tilePos.y) - _dir.ToVector2() * 5f - camPos);
        sprite.rotation = _dir.ToVector2().GetAngle();

        base.DrawSprites(sLeaser, rCam, timeStacker, camPos);
    }

    public override void AddToContainer(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, FContainer newContatiner)
    {
        newContatiner ??= rCam.ReturnFContainer("Foreground");
        foreach (var t in sLeaser.sprites)
        {
            newContatiner.AddChild(t);
        }

        base.AddToContainer(sLeaser, rCam, newContatiner);
    }

    public void SetPos(IntVector2 tilePos, IntVector2 dir)
    {
        _tilePos = tilePos;
        _dir = dir;

        IntVector2 perp = Custom.PerpIntVec(dir);

        PortalTiles = new[]
        {
            tilePos + perp,
            tilePos,
            tilePos - perp
        };
    }


    public readonly int portalID;
    private IntVector2 _tilePos;
    private IntVector2 _dir;
    
    public IntVector2[] PortalTiles { get; private set; }

}
