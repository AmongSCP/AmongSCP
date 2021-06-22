using Exiled.API.Extensions;
using HarmonyLib;
using UnityEngine;
using Mirror;
using Exiled.API.Features;

namespace AmongSCP.Patches
{
    [HarmonyPatch(typeof(FlickerableLightController), nameof(FlickerableLightController.RpcSetLightIntensity))]
    public static class ImposterLightsPatch
    {
        public static bool Prefix(FlickerableLightController __instance, float intensityMultiplier)
        {
            foreach (Player ply in Player.List)
            {
               var playerInfo = ply.GetInfo();

               if (playerInfo.Role == PlayerManager.Role.Imposter)
               {
                    MirrorExtensions.SendFakeTargetRpc(ply, __instance.netIdentity, typeof(FlickerableLightController), nameof(FlickerableLightController.RpcSetLightIntensity), 1f);
               }
               else
               {
                    MirrorExtensions.SendFakeTargetRpc(ply, __instance.netIdentity, typeof(FlickerableLightController), nameof(FlickerableLightController.RpcSetLightIntensity), intensityMultiplier);
               }
            }
            return false;
        }
    }
}
