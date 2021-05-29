using Exiled.API.Extensions;
using Exiled.API.Features;
using HarmonyLib;

namespace AmongSCP.Patches
{
    [HarmonyPatch(typeof(Inventory), nameof(Inventory.NetworkitemUniq), MethodType.Setter)]
    public static class SyncCurItemUniqPatch
    {
        public static bool Prefix(Inventory __instance, int value)
        {
            var player = Player.Get(__instance._hub);
            
            MirrorExtensions.SendFakeSyncVar(player, player.ReferenceHub.networkIdentity, typeof(Inventory), nameof(Inventory.NetworkitemUniq), value);
            
            __instance.itemUniq = value;
            return false;
        }
    }
}