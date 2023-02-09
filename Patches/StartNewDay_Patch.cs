using HarmonyLib;
using Kitchen;
using Unity.Entities;

namespace KitchenCustomDifficulty.Patches
{
    [HarmonyPatch]
    public static class StartNewDay_Patch
    {
        [HarmonyPatch(typeof(StartNewDay), "BecomeDay")]
        [HarmonyPrefix]
        public static bool BecomeDay_Prefix(StartNewDay __instance)
        {
            Main.LogInfo($"PrepEndPreference = {Main.RestartFromPrepEndPreference.Load(Main.MOD_GUID)}");
            if (!__instance.HasSingleton<SPracticeMode>() && Main.RestartFromPrepEndPreference.Load(Main.MOD_GUID) == 1)
            {
                Main.LogInfo("Saving at start of day.");
                Persistence.BackupWorld<WorldBackupSystem>(World.DefaultGameObjectInjectionWorld.EntityManager);
            }
            return true;
        }
    }
}
