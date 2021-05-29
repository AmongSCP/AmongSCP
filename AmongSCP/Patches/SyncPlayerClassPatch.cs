using Exiled.API.Extensions;
using Exiled.API.Features;
using HarmonyLib;

namespace AmongSCP.Patches
{
    [HarmonyPatch(typeof(CharacterClassManager), nameof(CharacterClassManager.NetworkCurClass), MethodType.Setter)]
    public static class SyncCurPlayerClassPatch
    {
        public static bool Prefix(CharacterClassManager __instance, RoleType value)
        {
            if (value != AmongSCP.Singleton.Config.CrewmateRole && value != AmongSCP.Singleton.Config.ImposterRole) return true;

            __instance.CurClass = value;

            //Tell the player what their class is.
            var player = Player.Get(__instance._hub);
            if (player != null)
            {
                MirrorExtensions.SendFakeSyncVar(player, player.ReferenceHub.networkIdentity, typeof(CharacterClassManager), nameof(CharacterClassManager.NetworkCurClass), (sbyte) value);
            }
            return false;
        }
    }
}