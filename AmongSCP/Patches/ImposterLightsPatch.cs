using Exiled.API.Extensions;
using HarmonyLib;
using UnityEngine;
using Mirror;
using Exiled.API.Features;

namespace AmongSCP.Patches
{
    [HarmonyPatch(typeof(FlickerableLightController), nameof(FlickerableLightController.ServerSetLightIntensity))]
    public static class ImposterLightsPatch
    {
        public static bool Prefix(FlickerableLightController __instance)
        {
            var networkIdentity = __instance.netIdentity;
            foreach(Player ply in EventHandlers.PlayerManager.AlivePlayers)
            {
                if(EventHandlers.PlayerManager.Imposters.Contains(ply))
                {
                    MirrorExtensions.SendFakeTargetRpc(ply, networkIdentity, typeof(FlickerableLightController), nameof(FlickerableLightController.ServerSetLightIntensity), 1f);
                }
                else
                {
                    MirrorExtensions.SendFakeTargetRpc(ply, networkIdentity, typeof(FlickerableLightController), nameof(FlickerableLightController.ServerSetLightIntensity), 0f);
                }
            }
            
            return false;
        }
    }
}
