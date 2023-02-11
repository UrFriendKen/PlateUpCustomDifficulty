using HarmonyLib;
using Kitchen;
using Unity.Entities;

namespace KitchenCustomDifficulty.Patches
{
    [HarmonyPatch]
    internal static class StartNewDay_Patch
    {
        [HarmonyPatch(typeof(StartNewDay), "BecomeDay")]
        [HarmonyPrefix]
        public static bool BecomeDay_Prefix(StartNewDay __instance)
        {
            if (!__instance.HasSingleton<SPracticeMode>() && Main.PrefManager.Get<int>(Main.RESTART_FROM_PREP_END_ID) == 1)
            {
                Main.LogInfo("Saving at start of day.");
                Persistence.BackupWorld<WorldBackupSystem>(World.DefaultGameObjectInjectionWorld.EntityManager);
            }
            return true;
        }
    }
}
