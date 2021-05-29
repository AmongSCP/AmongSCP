using Exiled.API.Features;
using HarmonyLib;

namespace AmongSCP.Patches
{
    [HarmonyPatch(typeof(Inventory), nameof(Inventory.NetworkitemUniq), MethodType.Setter)]
    public static class SyncCurItemUniqPatch
    {
        public static bool Prefix(Inventory __instance, int value)
        {
            var player = Player.Get(__instance.gameObject);

            if (player == null || !EventHandlers.PlayerManager.Imposters.Contains(player)) return true;

            __instance.itemUniq = value;
            return false;
        }
    }
}