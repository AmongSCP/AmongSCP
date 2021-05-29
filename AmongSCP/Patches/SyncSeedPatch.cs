using Exiled.API.Features;
using HarmonyLib;
using MapGeneration;

namespace AmongSCP.Patches
{
    [HarmonyPatch(typeof(SeedSynchronizer), nameof(SeedSynchronizer.Network_syncSeed), MethodType.Setter)]
    public static class SyncSeedPatch
    {
        public static bool Prefix(SeedSynchronizer __instance, int value)
        {
            value = 1378514975;
            return true;
        }
    }
}