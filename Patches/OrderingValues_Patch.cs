using HarmonyLib;
using KitchenData;

namespace KitchenCustomDifficulty.Patches
{
    [HarmonyPatch]
    internal static class OrderingValues_Patch
    {
        [HarmonyPatch(typeof(OrderingValues), nameof(OrderingValues.Default), MethodType.Getter)]
        [HarmonyPostfix]
        public static void Default_Postfix(ref OrderingValues __result)
        {
            float messMultipler = Main.PrefSysManager.Get<int>(Main.MESS_FACTOR_ID);
            if (messMultipler == -1)
            {
                messMultipler = Main.DefaultValuesDict[Main.MESS_FACTOR_ID];
            }

            __result.MessFactor *= messMultipler / 100f;
        }
    }
}
