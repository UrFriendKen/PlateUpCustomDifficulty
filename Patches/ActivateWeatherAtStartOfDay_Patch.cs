using HarmonyLib;
using Kitchen;
using System;
using System.Reflection;

namespace KitchenCustomDifficulty.Patches
{
    [HarmonyPatch]
    static class ActivateWeatherAtStartOfDay_Patch
    {
        public static MethodBase TargetMethod()
        {
            Type type = AccessTools.FirstInner(typeof(ActivateWeatherAtStartOfDay), t => t.Name.Contains("c__DisplayClass_OnUpdate_LambdaJob0"));
            return AccessTools.FirstMethod(type, method => method.Name.Contains("OriginalLambdaBody"));
        }

        public static bool Prefix(ref SWeatherPrecipitation weather)
        {
            int prefVal = Main.PrefSysManager.Get<int>(Main.WEATHER_ACTIVE_ID);
            if (prefVal == -2)
                return true;

            Main.LogInfo($"Overriding weather ({(prefVal == 1? "Enabled" : "Disabled")})");
            weather.IsActive = prefVal == 1;
            return false;
        }
    }
}
