﻿using HarmonyLib;

namespace AmongSCP.Patches
{
    [HarmonyPatch(typeof(Inventory), nameof(Inventory.Network_curItemSynced), MethodType.Setter)]
    public static class SyncCurItemPatch
    {
        public static bool Prefix(Inventory __instance, ItemType value)
        {
            //Log.Debug("SyncCurItemPatch");
            __instance._curItemSynced = value;
            return false;
        }
    }
}