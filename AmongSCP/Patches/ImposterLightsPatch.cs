using Exiled.API.Extensions;
using HarmonyLib;
using Exiled.API.Features;

namespace AmongSCP.Patches
{
    [HarmonyPatch(typeof(FlickerableLightController), nameof(FlickerableLightController.RpcSetLightIntensity))]
    public static class ImposterLightsPatch
    {
        public static bool Prefix(FlickerableLightController __instance, float intensityMultiplier)
        {
            foreach (var ply in Player.List)
            {
               var playerInfo = ply.GetInfo();

               MirrorExtensions.SendFakeTargetRpc(ply, __instance.netIdentity, typeof(FlickerableLightController), nameof(FlickerableLightController.RpcSetLightIntensity), playerInfo.Role == PlayerManager.Role.Imposter ? 1f : intensityMultiplier);
            }
            return false;
        }
    }
}
