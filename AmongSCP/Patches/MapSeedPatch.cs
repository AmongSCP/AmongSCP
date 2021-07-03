using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using MapGeneration;

namespace AmongSCP.Patches
{
    [HarmonyPatch(typeof(SeedSynchronizer), nameof(SeedSynchronizer.Start))]
    public static class MapSeedPatch
    {
        public static bool Prefix(SeedSynchronizer __instance)
        {
            __instance.Network_syncSeed = 1378514975;
            return false;
        }
    }
}
