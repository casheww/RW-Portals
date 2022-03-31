using System.Collections.Generic;
using RWCustom;
using UnityEngine;

namespace Portals;

public class PortalPair
{
    public PortalPair(Player player)
    {
        this.player = player;
        _playerLastRoom = player.room;
        _portals = new []
        {
            new Portal(0),
            new Portal(1)
        };
        _noTeleportList = new List<PhysicalObject>();
    }

    public void Update()
    {
        if (player.room == null) return;
        
        if (player.room != _playerLastRoom)
        {
            A.ClearFromRoom();
            B.ClearFromRoom();
            _playerLastRoom = player.room;
            _noTeleportList.Clear();
        }
        
        if (Input.GetKeyDown(KeyCode.D))
            SetPortal(0);
        else if (Input.GetKeyDown(KeyCode.F))
            SetPortal(1);

        _noTeleportList.RemoveAll(obj => !CheckInPortals(obj, out _));
    }
    
    private void SetPortal(int id)
    {
        if (!TryShootPortal(out IntVector2 tilePos, out IntVector2 dir))
            return;
        
        _portals[id].SetPos(player.room, tilePos, dir);

        PortalPlugin.Log.LogInfo($"set portal {player.playerState.playerNumber}:{id} at {tilePos}");
    }
    
    private bool TryShootPortal(out IntVector2 pos, out IntVector2 dirIntoPortal)
    {
        pos = player.room.GetTilePosition(player.firstChunk.pos);
        IntVector2 dir = GetThrowDirection(player);
        //IntVector2 dirPerp = Custom.PerpIntVec(dir);

        while (player.room.IsPositionInsideBoundries(pos))
        {
            pos += dir;
            
            foreach (PortalPair pp in PortalPlugin.PortalDict.Values)
            {
                if (pp.CheckInPortals(pos, out int existingPortal))
                {
                    _portals[existingPortal].ClearFromRoom();
                    
                    dirIntoPortal = dir;
                    return true;
                }
            }

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

    public bool CheckInPortals(PhysicalObject obj, out int id)
    {
        foreach (BodyChunk bc in obj.bodyChunks)
        {
            if (bc?.owner?.room == null)
                continue;

            if (CheckInPortals(bc, out id))
                return true;
        }

        id = -1;
        return false;
    }

    private bool CheckInPortals(BodyChunk bc, out int id)
        => CheckInPortals(bc.owner.room.GetTilePosition(bc.pos), out id);

    private bool CheckInPortals(IntVector2 pos, out int id)
    {
        for (int i = 0; i < _portals.Length; i++)
        {
            foreach (IntVector2 tile in _portals[i].portalTileCoords)
                if (tile == pos)
                {
                    id = i;
                    return true;
                }
        }

        id = -1;
        return false;
    }

    public void Teleport(PhysicalObject obj, int fromPortalIndex)
    {
        if (_noTeleportList.Contains(obj)) return;
        
        Portal fromPortal = _portals[fromPortalIndex];
        Portal toPortal = _portals[fromPortalIndex == 0 ? 1 : 0];

        if (toPortal.room == null) return;

        foreach (BodyChunk bc in obj.bodyChunks)
            TeleportChunk(bc, fromPortal, toPortal);

        _noTeleportList.Add(obj);
    }

    private void TeleportChunk(BodyChunk bc, Portal fromPortal, Portal toPortal)
    {
        Vector2 fromOrientation = Custom.PerpendicularVector(fromPortal.Dir.ToVector2());
        Vector2 toOrientation = Custom.PerpendicularVector(toPortal.Dir.ToVector2());
        
        float portalRotation = Custom.Angle(fromOrientation, toOrientation);
        float transmissionAngle = -Custom.Angle(fromPortal.Dir.ToVector2(), bc.vel);
        transmissionAngle += portalRotation - 180f;

        PortalPlugin.Log.LogInfo($"vel before : {bc.vel} :::   angle : {transmissionAngle}");
        bc.vel = Custom.RotateAroundOrigo(bc.vel, transmissionAngle);
        PortalPlugin.Log.LogInfo($"vel after  : {bc.vel}");
        var a = toPortal.pos;
        PortalPlugin.Log.LogInfo(a);
        bc.HardSetPosition(a);
    }

    public readonly Player player;
    private Room _playerLastRoom;
    private readonly Portal[] _portals;

    private List<PhysicalObject> _noTeleportList; 

    public Portal A => _portals[0];
    public Portal B => _portals[1];
}
