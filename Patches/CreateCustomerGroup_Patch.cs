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
            CCustomer customer = Main.instance.EntityManager.GetComponentData<CCustomer>(__result);

            int multiplier = Main.PrefSysManager.Get<int>(Main.CUSTOMER_SPEED_ID);
            if (multiplier == -1)
            {
                customer.Speed *= Main.DefaultValuesDict[Main.CUSTOMER_SPEED_ID] / 100f;
            }
            else
            {
                customer.Speed *= multiplier / 100f;
            }
            Main.instance.EntityManager.SetComponentData(__result, customer);
        }
    }
}
