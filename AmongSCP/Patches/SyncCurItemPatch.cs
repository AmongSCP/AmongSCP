using Exiled.API.Features;
using HarmonyLib;

namespace AmongSCP.Patches
{
    [HarmonyPatch(typeof(Inventory), nameof(Inventory.Network_curItemSynced), MethodType.Setter)]
    public static class SyncCurItemPatch
    {
        public static bool Prefix(Inventory __instance, ItemType value)
        {
            var player = Player.Get(__instance.gameObject);

            if (player == null || !EventHandlers.PlayerManager.Imposters.Contains(player)) return true;

            __instance._curItemSynced = value;
            return false;
        }
    }
}