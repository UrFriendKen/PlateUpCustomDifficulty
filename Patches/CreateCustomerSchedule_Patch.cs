using HarmonyLib;
using Kitchen;

namespace KitchenCustomDifficulty.Patches
{
    [HarmonyPatch]
    internal class CreateCustomerSchedule_Patch
    {
        public static bool Performed = false;

        [HarmonyPatch(typeof(CreateCustomerSchedule), "OnUpdate")]
        [HarmonyPrefix]
        static void OnUpdate_Prefix()
        {
            GroupSizeController.UseCustomKitchenParameters();
            Performed = true;
        }
    }
}
