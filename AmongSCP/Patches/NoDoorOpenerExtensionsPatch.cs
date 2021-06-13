using HarmonyLib;
using Interactables.Interobjects.DoorUtils;

namespace AmongSCP.Patches
{
    [HarmonyPatch(typeof(DoorEventOpenerExtension), nameof(DoorEventOpenerExtension.TriggerAction))]
    public static class NoDoorOpenerExtensionsPatch
    {
        public static bool Prefix(DoorEventOpenerExtension.OpenerEventType ev)
        {
            return false;
        }
    }
}