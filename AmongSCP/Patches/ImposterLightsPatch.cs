using Exiled.API.Extensions;
using HarmonyLib;
using Exiled.API.Features;

namespace AmongSCP.Patches
{
    [HarmonyPatch(typeof(FlickerableLightController), nameof(FlickerableLightController.Network_lightIntensityMultiplier), MethodType.Setter)]
    public static class ImposterLightsPatch
    {
        public static bool Prefix(FlickerableLightController __instance, float value)
        {
            foreach (var ply in Player.List)
            {
               var playerInfo = ply.GetInfo();

               MirrorExtensions.SendFakeSyncVar(ply, __instance.netIdentity, typeof(FlickerableLightController), nameof(FlickerableLightController.Network_lightIntensityMultiplier), playerInfo.Role == PlayerManager.Role.Imposter ? 1f : value);
            }
            return false;
        }
    }
}
