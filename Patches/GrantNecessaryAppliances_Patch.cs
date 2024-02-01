using HarmonyLib;
using Kitchen;

namespace KitchenCustomDifficulty.Patches
{
    [HarmonyPatch]
    internal class GrantNecessaryAppliances_Patch
    {
        [HarmonyPatch(typeof(GrantNecessaryAppliances), "OnUpdate")]
        [HarmonyPostfix]
        static void OnUpdate_Prefix()
        {
            GroupSizeController.UseCustomKitchenParameters();
        }

        [HarmonyPatch(typeof(GrantNecessaryAppliances), "OnUpdate")]
        [HarmonyPostfix]
        static void OnUpdate_Postfix()
        {
            GroupSizeController.ResetKitchenParameters();
        }
    }
}
