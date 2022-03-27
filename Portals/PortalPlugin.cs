using System.Collections.Generic;
using BepInEx;

namespace Portals;

[BepInPlugin("casheww.portals", nameof(Portals), "0.1.0")]
public sealed class PortalPlugin : BaseUnityPlugin
{
    public PortalPlugin()
    {
        Log = Logger;
        PortalDict = new Dictionary<Player, PortalPair>();
    }
    
    public void OnEnable() => Hooks.Hook();
    public void OnDisable() => Hooks.UnHook();
    
    public static void TryCreatePortal(Player player, int id)
    {
        if (!PortalDict.ContainsKey(player))
            PortalDict.Add(player, new PortalPair(player));
        
        PortalDict[player].SetPortal(id, player.room);
    }


    public static BepInEx.Logging.ManualLogSource Log { get; private set; }

    public static Dictionary<Player, PortalPair> PortalDict { get; private set; }

}
