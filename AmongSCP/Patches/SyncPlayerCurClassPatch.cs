using Exiled.API.Features;
using HarmonyLib;

namespace AmongSCP.Patches
{
    [HarmonyPatch(typeof(CharacterClassManager), nameof(CharacterClassManager.NetworkCurClass), MethodType.Setter)]
    public static class SyncCurPlayerClassPatch
    {
        // This disguises the player every time their class is changed.
        public static bool Prefix(CharacterClassManager __instance, RoleType value)
        {
            if (value != AmongSCP.Singleton.Config.CrewmateRole && value != AmongSCP.Singleton.Config.ImposterRole) return true;

            var player = Player.Get(__instance._hub);
            Util.ChangeOutfit(__instance.netIdentity, player.Role.Type);
            
            __instance.CurClass = value;
            return false;
        }
    }
}