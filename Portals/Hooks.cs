using Mono.Cecil.Cil;
using MonoMod.Cil;
using RWCustom;
using UnityEngine;

namespace Portals;

public static class Hooks
{
    public static void Hook()
    {
        On.Player.Update += Player_Update;
        IL.BodyChunk.CheckHorizontalCollision += BodyChunk_CheckHorizontalCollision;
        IL.BodyChunk.CheckVerticalCollision += BodyChunk_CheckVerticalCollision;
    }

    public static void UnHook()
    {
        On.Player.Update -= Player_Update;
        IL.BodyChunk.CheckHorizontalCollision -= BodyChunk_CheckHorizontalCollision;
        IL.BodyChunk.CheckVerticalCollision -= BodyChunk_CheckVerticalCollision;
    }
    
    private static void Player_Update(On.Player.orig_Update orig, Player self, bool eu)
    {
        orig(self, eu);

        if (Input.GetKeyDown(KeyCode.D))
            PortalPlugin.TryCreatePortal(self, 0);
        else if (Input.GetKeyDown(KeyCode.F))
            PortalPlugin.TryCreatePortal(self, 1);
    }
    
    private static void BodyChunk_CheckHorizontalCollision(ILContext il)
    {
        try
        {
            PortalPlugin.Log.LogInfo("BodyChunk_CheckHorizontalCollision il edit 0/2");
            ILCursor c = new ILCursor(il);
            ILLabel branch = null;
            
            bool done = false;
            while (!done)
            {
                c.GotoNext(x => x.MatchLdarg(0));
                c.Index++;
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
                c.Index++;
                
                done = true;
            }

            c.Emit(OpCodes.Br, branch);
            PortalPlugin.Log.LogInfo("BodyChunk_CheckHorizontalCollision il edit 1/2");

            c.Index = 0;
            branch = null;
            done = false;
            while (!done)
            {
                c.GotoNext(x => x.MatchLdarg(0));
                c.Index++;
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
                c.Index++;
                
                done = true;
            }

            c.Emit(OpCodes.Br, branch);
            PortalPlugin.Log.LogInfo("BodyChunk_CheckHorizontalCollision il edit 2/2");
        }
        catch (System.Exception e)
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
            
            bool done = false;
            while (!done)
            {
                c.GotoNext(x => x.MatchLdarg(0));
                c.Index++;
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
                c.Index++;
                
                done = true;
            }
            
            c.Emit(OpCodes.Br, branch);
            PortalPlugin.Log.LogInfo("BodyChunk_CheckVerticalCollision il edit 1/2");
            
            c.Index = 0;
            branch = null;
            done = false;
            while (!done)
            {
                c.GotoNext(x => x.MatchLdarg(0));
                c.Index++;
                if (!c.Next.MatchLdloc(15)) continue;
                c.Index++;
                if (!c.Next.MatchLdloc(14)) continue;
                c.Index++;
                if (!c.Next.MatchCallvirt<BodyChunk>("SolidFloor")) continue;
                c.Index++;
                if (!c.Next.MatchBrfalse(out branch)) continue;
                c.Index++;

                done = true;
            }

            c.Emit(OpCodes.Br, branch);
            PortalPlugin.Log.LogInfo("BodyChunk_CheckVerticalCollision il edit 2/2");
        }
        catch (System.Exception e)
        {
            PortalPlugin.Log.LogError(e);
        }
    }

    private static void MovePhysicalObject(PhysicalObject obj, IntVector2 tilePos)
    {
        foreach (BodyChunk bc in obj.bodyChunks)
            bc.HardSetPosition(tilePos.ToVector2() * 20f);
    }

}
