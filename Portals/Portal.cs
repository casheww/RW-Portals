using RWCustom;
using UnityEngine;

namespace Portals;

public class Portal : CosmeticSprite
{
    public Portal(int id)
    {
        portalID = id;
        hide = true;
        portalTileCoords = new IntVector2[portalHeight];
        _displacedTiles = new Room.Tile[portalHeight];

        TilePos = new IntVector2(-1, -1);
        Dir = new IntVector2(0, 1);
    }

    public void ClearFromRoom()
    {
        if (room == null) return;
        
        RestoreTiles();
        room.RemoveObject(this);
        hide = true;
    }

    public void SetPos(Room newRoom, IntVector2 tilePos, IntVector2 dir)
    {
        ClearFromRoom();
        newRoom.AddObject(this);
        
        TilePos = tilePos;
        Dir = dir;

        IntVector2 perp = Custom.PerpIntVec(dir);
        portalTileCoords[0] = tilePos + perp;
        portalTileCoords[1] = tilePos;
        portalTileCoords[2] = tilePos - perp;

        DisplaceTiles();

        hide = false;
    }
    
    private void RestoreTiles()
    {
        for (int i = 0; i < portalHeight; i++)
        {
            IntVector2 coords = portalTileCoords[i];
            
            PortalPlugin.Log.LogInfo(room == null);
            PortalPlugin.Log.LogInfo(room.Tiles == null);

            room.Tiles[coords.x, coords.y] = _displacedTiles[i];
        }
    }

    private void DisplaceTiles()
    {
        for (int i = 0; i < portalHeight; i++)
        {
            IntVector2 coords = portalTileCoords[i];
            
            _displacedTiles[i] = room.GetTile(coords);
            room.Tiles[coords.x, coords.y] =
                new Room.Tile(coords.x, coords.y, Room.Tile.TerrainType.Air, false, false, false, 0, 0);
        }
    }
    
    #region drawable
    
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
        if (hide) return;
        
        FSprite sprite = sLeaser.sprites[0];
        sprite.color = portalID == 0 ? new Color(0.7f, 0.2f, 0.2f) : new Color(0.2f, 0.6f, 0.2f);
        
        sprite.scaleX = 0.5f;
        sprite.scaleY = 3f;
        sprite.SetPosition(rCam.room.MiddleOfTile(TilePos.x, TilePos.y) - Dir.ToVector2() * 5f - camPos);
        sprite.rotation = Dir.ToVector2().GetAngle();

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
    
    #endregion drawable


    public readonly int portalID;
    public bool hide;
    public IntVector2 TilePos { get; private set; }
    public IntVector2 Dir { get; private set; }

    private const int portalHeight = 3;
    public readonly IntVector2[] portalTileCoords;
    private readonly Room.Tile[] _displacedTiles;

}
