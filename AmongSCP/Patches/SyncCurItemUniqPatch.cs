using Exiled.API.Extensions;
using Exiled.API.Features;
using HarmonyLib;
using InventorySystem;

namespace AmongSCP.Patches
{
    [HarmonyPatch(typeof(Inventory), nameof(Inventory.NetworkCurItem), MethodType.Setter)]
    public static class SyncCurItemUniqPatch
    {
        public static bool Prefix(Inventory __instance, int value)
        {
            var player = Player.Get(__instance._hub);
            
            player.SendFakeSyncVar(player.ReferenceHub.networkIdentity, typeof(Inventory), nameof(Inventory.NetworkCurItem), value);
            
            __instance.itemUniq = value;
            return false;
        }
    }
}