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
            return false;
        }
    }
}