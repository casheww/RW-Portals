using RWCustom;
using UnityEngine;

namespace Portals;

public class Portal : CosmeticSprite
{
    public Portal(Player player, int id, IntVector2 tilePos)
    {
        this.player = player;
        room = player.room;
        portalID = id;
        this.tilePos = tilePos;
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
        sprite.SetPosition(rCam.room.MiddleOfTile(tilePos.x, tilePos.y) - camPos);
        sprite.rotation = dir.ToVector2().GetAngle();

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
    

    public readonly Player player;
    public readonly int portalID;
    public IntVector2 tilePos;
    public IntVector2 dir;

}
