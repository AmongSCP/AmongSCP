using Exiled.API.Extensions;
using Exiled.API.Features;
using HarmonyLib;
using InventorySystem;
using InventorySystem.Items;

namespace AmongSCP.Patches
{
    [HarmonyPatch(typeof(Inventory), nameof(Inventory.NetworkCurItem), MethodType.Setter)]
    public static class SyncCurItemUniqPatch
    {
        // This patch is done so that an imposter with a gun sees their own gun, and the server recognizes that they
        // have the gun, but other players do not see it.
        public static bool Prefix(Inventory __instance, ItemIdentifier value)
        {
            var player = Player.Get(__instance._hub);
            
            player.SendFakeSyncVar(player.ReferenceHub.networkIdentity, typeof(Inventory), nameof(Inventory.NetworkCurItem), value);
            
            __instance.CurItem = value;
            return false;
        }
    }
}