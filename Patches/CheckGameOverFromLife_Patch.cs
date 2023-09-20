using HarmonyLib;
using Kitchen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KitchenCustomDifficulty.Patches
{
    [HarmonyPatch]
    static class CheckGameOverFromLife_Patch
    {

        [HarmonyPatch(typeof(CheckGameOverFromLife), "RescuedByDay")]
        [HarmonyPrefix]
        static bool RescuedByDay_Prefix(ref bool __result)
        {
            __result = PatchController.CustomOfferRestart(out bool shouldRunOriginal);
            return shouldRunOriginal;
        }
    }
}
