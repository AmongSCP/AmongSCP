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
            
            Util.ChangeOutfit(__instance.netIdentity);
            
            __instance.CurClass = value;
            return false;
        }
    }
}