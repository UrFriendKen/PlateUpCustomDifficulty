using HarmonyLib;
using Kitchen;
using Unity.Entities;

namespace KitchenCustomDifficulty.Patches
{
    [HarmonyPatch]
    static class CreateCustomerGroup_Patch
    {
        [HarmonyPatch(typeof(CreateCustomerGroup), "NewCustomer")]
        [HarmonyPostfix]
        static void NewCustomer_Postfix(ref Entity __result)
        {
            PatchController.UpdateCustomerSpeed(__result);
        }
    }
}
