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
    internal class CreateCustomerSchedule_Patch
    {
        [HarmonyPatch(typeof(CreateCustomerSchedule), "OnUpdate")]
        [HarmonyPrefix]
        static void OnUpdate_Prefix()
        {
            GroupSizeController.UseCustomKitchenParameters();
        }

        [HarmonyPatch(typeof(CreateCustomerSchedule), "OnUpdate")]
        [HarmonyPostfix]
        static void OnUpdate_Postfix()
        {
            GroupSizeController.ResetKitchenParameters();
        }
    }
}
