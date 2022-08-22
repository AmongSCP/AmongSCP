using HarmonyLib;

namespace AmongSCP.Patches
{
    [HarmonyPatch(typeof(SCPStats.Config), nameof(SCPStats.Config.EnableHats), MethodType.Getter)]
    public class SCPStatsHatPatch
    {
        public static bool Prefix(ref bool __result)
        {
            __result = false;
            return false;
        }
    }
}