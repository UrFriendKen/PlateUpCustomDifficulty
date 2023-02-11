using HarmonyLib;
using Kitchen;
using KitchenLib.Utils;
using System.Reflection;

namespace KitchenCustomDifficulty.Patches
{
    [HarmonyPatch]
    internal static class ProgressionHelpers_Patch
    {
        private static MethodInfo mProgressionSetDayLengthOnUpdate = ReflectionUtils.GetMethod<ProgressionSetDayLength>("OnUpdate", BindingFlags.NonPublic | BindingFlags.Instance);

        [HarmonyPatch(typeof(ProgressionHelpers), nameof(ProgressionHelpers.GetDayLength))]
        [HarmonyPostfix]
        public static void GetDayLength_Postfix(ref float __result)
        {
            int multiplier = Main.PrefManager.Get<int>(Main.DAY_LENGTH_ID);

            if (multiplier == -2)
            {
                return;
            }
            else if (multiplier == -1)
            {
                __result *= Main.DefaultValuesDict[Main.DAY_LENGTH_ID] / 100f;
            }
            else
            {
                __result *= multiplier / 100f;
            }
        }
    }
}
