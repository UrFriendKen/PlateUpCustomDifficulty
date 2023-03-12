using HarmonyLib;
using Kitchen;

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
    }
}
