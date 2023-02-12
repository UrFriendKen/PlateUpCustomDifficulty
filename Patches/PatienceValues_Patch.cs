using HarmonyLib;
using KitchenData;

namespace KitchenCustomDifficulty.Patches
{
    [HarmonyPatch]
    internal static class PatienceValues_Patch
    {
        [HarmonyPatch(typeof(PatienceValues), nameof(PatienceValues.Default), MethodType.Getter)]
        [HarmonyPostfix]
        public static void Default_Postfix(ref PatienceValues __result)
        {
            if (Main.PrefManager.Get<int>(Main.PHASE_PATIENCE_ENABLED_ID) == 1)
            {
                __result.Thinking *= GetMultiplier(Main.ORDER_THINKING_ID);
                __result.Eating *= GetMultiplier(Main.ORDER_EATING_ID);
                __result.Seating *= GetMultiplier(Main.PATIENCE_SEATING_ID);
                __result.Service *= GetMultiplier(Main.PATIENCE_SERVICE_ID);
                __result.WaitForFood *= GetMultiplier(Main.PATIENCE_WAITFORFOOD_ID);
                __result.GetFoodDelivered *= GetMultiplier(Main.PATIENCE_DELIVERY_ID);
                __result.FoodDeliverBonus *= GetMultiplier(Main.PATIENCE_DELIVERY_BOOST_ID);
            }
        }

        private static float GetMultiplier(string preferenceID)
        {
            float prefVal = Main.PrefManager.Get<int>(preferenceID);
            if (prefVal == -1)
            {
                prefVal = Main.DefaultValuesDict[preferenceID];
            }
            return prefVal / 100f;
        }
    }
}
