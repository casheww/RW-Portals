using UnityEngine;

namespace Portals;

public static class Hooks
{
    public static void Hook()
    {
        On.Player.Update += Player_Update;
    }

    public static void UnHook()
    {
        On.Player.Update -= Player_Update;
    }
    
    private static void Player_Update(On.Player.orig_Update orig, Player self, bool eu)
    {
        orig(self, eu);

        if (Input.GetKeyDown(KeyCode.D))
            PortalPlugin.TryCreatePortal(self, 0);
        else if (Input.GetKeyDown(KeyCode.F))
            PortalPlugin.TryCreatePortal(self, 1);
    }

}
