using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using static HarmonyLib.AccessTools;
using InventorySystem.Items.Pickups;

namespace AmongSCP.Patches
{
    [HarmonyPatch(typeof(SCPStats.Hats.HatPlayerComponent), "MoveHat")]
    public class SCPStatsHatPickupPatch
    {
        // This changes every PickupSyncInfo.Locked = true to PickupSyncInfo.Locked = false.
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions,
            ILGenerator generator)
        {
            CodeInstruction toCheck = null;

            foreach (var codeInstruction in instructions)
            {
                if (toCheck == null)
                {
                    toCheck = codeInstruction;
                    continue;
                }

                yield return codeInstruction.operand is MethodInfo info && info.Name == "set_Locked"
                    ? new CodeInstruction(OpCodes.Ldc_I4_0)
                    : toCheck;
                toCheck = null;
                
                yield return codeInstruction;
            }

            if (toCheck != null)
                yield return toCheck;
        }
    }
}