using HarmonyLib;
using Kitchen;
using KitchenCustomDifficulty.Views;
using UnityEngine;

namespace KitchenCustomDifficulty.Patches
{
    [HarmonyPatch]
    public static class LocalViewRouter_Patch
    {
        [HarmonyPatch(typeof(LocalViewRouter), "GetPrefab")]
        [HarmonyPrefix]
        public static bool GetPrefab_Prefix(ref GameObject __result, ViewType view_type)
        {
            if (view_type == (ViewType)Main.PLAYER_COLLISION_VIEW_ID)
            {
                GameObject viewObject = new GameObject();
                viewObject.AddComponent<PlayerCollisionView>();
                __result = viewObject;
                return false;
            }
            return true;
        }
    }
}
