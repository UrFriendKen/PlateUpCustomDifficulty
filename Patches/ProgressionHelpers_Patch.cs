using HarmonyLib;
using Kitchen;
using System.Reflection;

namespace KitchenCustomDifficulty.Patches
{
    [HarmonyPatch]
    internal static class ProgressionHelpers_Patch
    {
        private static MethodInfo mProgressionSetDayLengthOnUpdate = typeof(ProgressionSetDayLength).GetMethod("OnUpdate", BindingFlags.NonPublic | BindingFlags.Instance);

        [HarmonyPatch(typeof(ProgressionHelpers), nameof(ProgressionHelpers.GetDayLength))]
        [HarmonyPostfix]
        public static void GetDayLength_Postfix(ref float __result)
        {
            int multiplier = Main.PrefSysManager.Get<int>(Main.DAY_LENGTH_ID);

            if (multiplier == -2)
            {
                return;
            }
            else if (multiplier == -1)
            {
                multiplier = Main.DefaultValuesDict[Main.DAY_LENGTH_ID];
            }
            
            if (multiplier == 0)
            {
                __result = float.Epsilon;
            }
            else
            {
                __result *= multiplier / 100f;
            }
        }
    }
}
