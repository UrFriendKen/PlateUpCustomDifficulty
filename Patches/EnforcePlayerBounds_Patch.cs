using HarmonyLib;
using Kitchen;

namespace KitchenCustomDifficulty.Patches
{
    [HarmonyPatch]
    static class EnforcePlayerBounds_Patch
    {
        [HarmonyPatch(typeof(EnforcePlayerBounds), "OnUpdate")]
        [HarmonyPrefix]
        static bool OnUpdate_Prefix()
        {
            return false;
        }
    }
}
