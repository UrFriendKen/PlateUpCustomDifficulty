using HarmonyLib;

namespace KitchenCustomDifficulty.Patches
{
    [HarmonyPatch]
    static class CheckGameOverFromLife_Patch
    {
        [HarmonyPatch(typeof(CheckGameOverFromLife), "OnUpdate")]
        [HarmonyPrefix]
        static bool OnUpdate_Prefix()
        {
            return false;
        }
    }
}
