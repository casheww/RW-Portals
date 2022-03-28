using System;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using RWCustom;

namespace Portals;

public static class Hooks
{
    public static void Hook()
    {
        On.Player.ctor += Player_ctor;
        On.Player.Update += Player_Update;
        IL.BodyChunk.CheckHorizontalCollision += BodyChunk_CheckHorizontalCollision;
        IL.BodyChunk.CheckVerticalCollision += BodyChunk_CheckVerticalCollision;
    }

    public static void UnHook()
    {
        On.Player.ctor -= Player_ctor;
        On.Player.Update -= Player_Update;
        IL.BodyChunk.CheckHorizontalCollision -= BodyChunk_CheckHorizontalCollision;
        IL.BodyChunk.CheckVerticalCollision -= BodyChunk_CheckVerticalCollision;
    }

    private static void Player_ctor(On.Player.orig_ctor orig, Player self, AbstractCreature abstractcreature, World world)
    {
        orig(self, abstractcreature, world);
        
        if (!PortalPlugin.PortalDict.ContainsKey(self))
            PortalPlugin.PortalDict.Add(self, new PortalPair(self));
    }

    private static void Player_Update(On.Player.orig_Update orig, Player self, bool eu)
    {
        orig(self, eu);
        PortalPlugin.PortalDict[self].Update();

        foreach (BodyChunk bc in self.bodyChunks)
        {
            foreach (PortalPair pp in PortalPlugin.PortalDict.Values)
            {
                foreach (IntVector2 tile in pp.A.portalTileCoords)
                    if (tile == bc?.owner?.room?.GetTilePosition(bc.pos))
                    { 
                        MovePhysicalObject(self, pp.B.TilePos);
                        return;
                    }

                foreach (IntVector2 tile in pp.B.portalTileCoords)
                    if (tile == bc?.owner?.room?.GetTilePosition(bc.pos))
                    {
                        MovePhysicalObject(self, pp.A.TilePos);
                        return;
                    }
            }
        }
    }

    private static void BodyChunk_CheckHorizontalCollision(ILContext il)
    {
        try
        {
            PortalPlugin.Log.LogInfo("BodyChunk_CheckHorizontalCollision il edit 0/2");
            ILCursor c = new ILCursor(il);
            ILLabel branch = null;
            int matchIndex = 0;
            
            bool done = false;
            while (!done)
            {
                c.GotoNext(x => x.MatchLdarg(0));
                matchIndex = c.Index++;
                if (!c.Next.MatchCallvirt<BodyChunk>("get_owner")) continue;
                c.Index++;
                if (!c.Next.MatchLdfld<UpdatableAndDeletable>("room")) continue;
                c.Index++;
                if (!c.Next.MatchLdloc(7)) continue;
                c.Index++;
                if (!c.Next.MatchLdloc(8)) continue;
                c.Index++;
                if (!c.Next.MatchCallvirt<Room>("GetTile")) continue;
                c.Index++;
                if (!c.Next.MatchCallvirt<Room.Tile>("get_Terrain")) continue;
                c.Index++;
                if (!c.Next.MatchLdcI4(out _)) continue;
                c.Index++;
                if (!c.Next.MatchBneUn(out branch)) continue;
                
                done = true;
            }

            EmitPortalCheck(c, matchIndex, branch);
            PortalPlugin.Log.LogInfo("BodyChunk_CheckHorizontalCollision il edit 1/2");

            c.Index = 0;
            branch = null;
            done = false;
            while (!done)
            {
                c.GotoNext(x => x.MatchLdarg(0));
                matchIndex = c.Index++;
                if (!c.Next.MatchCallvirt<BodyChunk>("get_owner")) continue;
                c.Index++;
                if (!c.Next.MatchLdfld<UpdatableAndDeletable>("room")) continue;
                c.Index++;
                if (!c.Next.MatchLdloc(14)) continue;
                c.Index++;
                if (!c.Next.MatchLdloc(15)) continue;
                c.Index++;
                if (!c.Next.MatchCallvirt<Room>("GetTile")) continue;
                c.Index++;
                if (!c.Next.MatchCallvirt<Room.Tile>("get_Terrain")) continue;
                c.Index++;
                if (!c.Next.MatchLdcI4(out _)) continue;
                c.Index++;
                if (!c.Next.MatchBneUn(out branch)) continue;
                
                done = true;
            }

            EmitPortalCheck(c, matchIndex, branch);
            PortalPlugin.Log.LogInfo("BodyChunk_CheckHorizontalCollision il edit 2/2");
        }
        catch (Exception e)
        {
            PortalPlugin.Log.LogError(e);
        }
    }
    
    private static void BodyChunk_CheckVerticalCollision(ILContext il)
    {
        try
        {
            PortalPlugin.Log.LogInfo("BodyChunk_CheckVerticalCollision il edit 0/2");
            ILCursor c = new ILCursor(il);
            ILLabel branch = null;
            int indexToEdit = 0;
            
            bool done = false;
            while (!done)
            {
                c.GotoNext(x => x.MatchLdarg(0));
                indexToEdit = c.Index++;
                if (!c.Next.MatchCallvirt<BodyChunk>("get_owner")) continue;
                c.Index++;
                if (!c.Next.MatchLdfld<UpdatableAndDeletable>("room")) continue;
                c.Index++;
                if (!c.Next.MatchLdloc(8)) continue;
                c.Index++;
                if (!c.Next.MatchLdloc(7)) continue;
                c.Index++;
                if (!c.Next.MatchCallvirt<Room>("GetTile")) continue;
                c.Index++;
                if (!c.Next.MatchCallvirt<Room.Tile>("get_Terrain")) continue;
                c.Index++;
                if (!c.Next.MatchLdcI4(out _)) continue;
                c.Index++;
                if (!c.Next.MatchBneUn(out branch)) continue;
                
                done = true;
            }
            
            EmitPortalCheck(c, indexToEdit, branch);
            PortalPlugin.Log.LogInfo("BodyChunk_CheckVerticalCollision il edit 1/2");
            
            c.Index = 0;
            branch = null;
            done = false;
            while (!done)
            {
                c.GotoNext(x => x.MatchLdarg(0));
                indexToEdit = c.Index++;
                if (!c.Next.MatchLdloc(15)) continue;
                c.Index++;
                if (!c.Next.MatchLdloc(14)) continue;
                c.Index++;
                if (!c.Next.MatchCallvirt<BodyChunk>("SolidFloor")) continue;
                c.Index++;
                if (!c.Next.MatchBrfalse(out branch)) continue;

                done = true;
            }

            EmitPortalCheck(c, indexToEdit, branch);
            PortalPlugin.Log.LogInfo("BodyChunk_CheckVerticalCollision il edit 2/2");
        }
        catch (Exception e)
        {
            PortalPlugin.Log.LogError(e);
        }
    }

    private static void EmitPortalCheck(ILCursor c, int beforeIndex, ILLabel branch)
    {
        c.Index = beforeIndex + 1;
        
        c.EmitDelegate<Func<BodyChunk, bool>>(bc =>
        {
            foreach (PortalPair pp in PortalPlugin.PortalDict.Values)
            {
                foreach (IntVector2 tile in pp.A.portalTileCoords)
                    if (tile == bc?.owner?.room?.GetTilePosition(bc.pos))
                        return true;

                foreach (IntVector2 tile in pp.B.portalTileCoords)
                    if (tile == bc?.owner?.room?.GetTilePosition(bc.pos))
                        return true;
            }

            return false;
        });
        c.Emit(OpCodes.Brtrue, branch);
        
        c.Emit(OpCodes.Ldarg_0);
    }

    private static void MovePhysicalObject(PhysicalObject obj, IntVector2 tilePos)
    {
        foreach (BodyChunk bc in obj.bodyChunks)
            bc.HardSetPosition(tilePos.ToVector2() * 20f);
        
        if (obj is Creature creature)
            foreach (Creature.Grasp grasp in creature.grasps)
                if (grasp != null)
                    MovePhysicalObject(grasp.grabbed, tilePos);
    }

}
