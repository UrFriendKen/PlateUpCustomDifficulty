using HarmonyLib;
using KitchenData;
using System;

namespace KitchenCustomDifficulty.Patches
{
    [HarmonyPatch]
    internal static class DecorationValues_Patch
    {
        [HarmonyPatch(typeof(DecorationValues), nameof(DecorationValues.GetBonusLevel), new Type[] { typeof(DecorationType) } )]
        [HarmonyPrefix]
        public static bool GetBonusLevel_Prefix(ref int __result, DecorationType t)
        {
            int decorationValue = -2;
            switch (t)
            {
                case DecorationType.Exclusive:
                    decorationValue = GetValue(Main.DECORATION_EXCLUSIVE_ID);
                    break;
                case DecorationType.Affordable:
                    decorationValue = GetValue(Main.DECORATION_AFFORDABLE_ID);
                    break;
                case DecorationType.Charming:
                    decorationValue = GetValue(Main.DECORATION_CHARMING_ID);
                    break;
                case DecorationType.Formal:
                    decorationValue = GetValue(Main.DECORATION_FORMAL_ID);
                    break;
                default:
                    break;
            }
            if (decorationValue > -2)
            {
                __result = decorationValue;
                return false;
            }
            return true;
        }

        private static int GetValue(string preferenceID)
        {
            int prefVal = Main.PrefSysManager.Get<int>(preferenceID);
            if (prefVal == -1)
            {
                prefVal = Main.DefaultValuesDict[preferenceID];
            }
            return prefVal;
        }
    }
}
