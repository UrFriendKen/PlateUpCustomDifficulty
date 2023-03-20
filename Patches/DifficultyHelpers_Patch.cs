using HarmonyLib;
using Kitchen;

namespace KitchenCustomDifficulty.Patches
{
    [HarmonyPatch(typeof(DifficultyHelpers))]
    internal class DifficultyHelpers_Patch
    {
        [HarmonyPostfix]
        [HarmonyPatch(nameof(DifficultyHelpers.CustomerChangePerPoint), MethodType.Getter)]
        public static void CustomerChangePerPoint_Postfix(ref float __result)
        {
            int prefVal = Main.PrefSysManager.Get<int>(Main.CARD_CUSTOMER_CHANGE_PER_POINT_ID);

            if (prefVal == -1)
                __result = Main.DefaultValuesDict[Main.CARD_CUSTOMER_CHANGE_PER_POINT_ID];
            else
                __result = (float)prefVal;
        }

        [HarmonyPriority(200)]
        [HarmonyPostfix]
        [HarmonyPatch(nameof(DifficultyHelpers.TotalShopCount))]
        public static void TotalShopCount_Postfix(ref int __result)
        {
            int prefVal = Main.PrefSysManager.Get<int>(Main.SHOP_TOTAL_APPLIANCE_BLUEPRINTS_ID);

            if (prefVal == -1)
                __result = Main.DefaultValuesDict[Main.SHOP_TOTAL_APPLIANCE_BLUEPRINTS_ID];
            else
                __result = prefVal;
        }

        [HarmonyPriority(200)]
        [HarmonyPostfix]
        [HarmonyPatch(nameof(DifficultyHelpers.StapleCount))]
        public static void StapleCount_Postfix(ref int __result)
        {
            int prefVal = Main.PrefSysManager.Get<int>(Main.SHOP_STAPLE_BLUEPRINTS_ID);

            if (prefVal == -2)
                return;

            if (prefVal == -1)
                __result = Main.DefaultValuesDict[Main.SHOP_STAPLE_BLUEPRINTS_ID];
            else
                __result = prefVal;
        }
        
        [HarmonyPriority(200)]
        [HarmonyPostfix]
        [HarmonyPatch(nameof(DifficultyHelpers.BaseUpgradedShopChance))]
        public static void BaseUpgradedShopChance_Postfix(ref float __result)
        {
            int prefVal = Main.PrefSysManager.Get<int>(Main.SHOP_UPGRADED_CHANCE_ID);

            if (prefVal == -2)
                return;

            if (prefVal == -1)
                __result = Main.DefaultValuesDict[Main.SHOP_UPGRADED_CHANCE_ID] / 100f;
            else
                __result = prefVal / 100f;
        }

        /*
        [HarmonyPriority(200)]
        [HarmonyPostfix]
        [HarmonyPatch(nameof(DifficultyHelpers.IncreasedRerollCost))]
        public static void IncreasedRerollCost_Postfix(ref int __result, int cost)
        {
            int prefVal = Main.PrefSysManager.Get<int>(Main.SHOP_INCREASE_REROLL_COST_ID);

            if (prefVal == -1)
                __result = cost + Main.SHOP_INCREASE_REROLL_COST_INITIAL;
            else
                __result = cost + prefVal;
        }
        */

        [HarmonyPriority(200)]
        [HarmonyPostfix]
        [HarmonyPatch(nameof(DifficultyHelpers.FireSpreadModifier))]
        public static void FireSpreadModifier_Postfix(ref float __result)
        {
            int prefVal = Main.PrefSysManager.Get<int>(Main.FIRE_SPREAD_ID);

            if (prefVal == -1)
                __result = Main.DefaultValuesDict[Main.FIRE_SPREAD_ID]/100f;
            else
                __result = prefVal/100f;
        }

        

        [HarmonyPriority(200)]
        [HarmonyPostfix]
        [HarmonyPatch(nameof(DifficultyHelpers.MoneyRewardPlayerModifier))]
        public static void MoneyRewardPlayerModifier_Postfix(ref float __result, int player_count)
        {
            int isCustomCustomerCountOn = Main.PrefSysManager.Get<int>(Main.PLAYER_CUSTOMERS_ENABLED_ID);
            if (isCustomCustomerCountOn > -2)
            {
                if (player_count == 1)
                    __result = 1.25f;
            }
        }

        [HarmonyPriority(200)]
        [HarmonyPostfix]
        [HarmonyPatch(nameof(DifficultyHelpers.CustomerPlayersRateModifier))]
        public static void CustomerPlayersRateModifier_Postfix(ref float __result, int player_count)
        {
            int enabled = Main.PrefSysManager.Get<int>(Main.PLAYER_CUSTOMERS_ENABLED_ID);
            if (enabled == 0)
                return;

            int multiplier = Main.PrefSysManager.Get<int>(Main.CUSTOMERS_PER_PLAYER_ID);
            if (multiplier == -1)
                multiplier = Main.DefaultValuesDict[Main.CUSTOMERS_PER_PLAYER_ID];

            int baseVal = Main.PrefSysManager.Get<int>(Main.BASE_PLAYER_CUSTOMERS_ID);
            if (baseVal == -1)
                baseVal = Main.DefaultValuesDict[Main.BASE_PLAYER_CUSTOMERS_ID];
            
            int playerMultiplier = (player_count < 2) ? 0 : (player_count - 1);
            float customerPlayersRateModifier = baseVal / 100f
                                                + (playerMultiplier * multiplier) /100f;
            
            __result = customerPlayersRateModifier;
        }

        [HarmonyPriority(1600)]
        [HarmonyPostfix]
        [HarmonyPatch(nameof(DifficultyHelpers.PatiencePlayerCountModifier))]
        public static void PatiencePlayerCountModifier_Postfix(ref float __result, int player_count)
        {
            int enabled = Main.PrefSysManager.Get<int>(Main.PLAYER_PATIENCE_ENABLED_ID);
            if (enabled == 0)
                return;

            int multiplier = Main.PrefSysManager.Get<int>(Main.PATIENCE_PER_PLAYER_ID);
            if (multiplier == -1)
                multiplier = Main.DefaultValuesDict[Main.PATIENCE_PER_PLAYER_ID];

            int baseVal = Main.PrefSysManager.Get<int>(Main.BASE_PLAYER_PATIENCE_ID);
            if (baseVal == -1)
                baseVal = Main.DefaultValuesDict[Main.BASE_PLAYER_PATIENCE_ID];

            int playerMultiplier = (player_count < 2) ? 0 : (player_count - 1);
            float patiencePlayersRateModifier = baseVal / 100f
                                                + (playerMultiplier * multiplier) / 100f;

            __result = patiencePlayersRateModifier;
        }
    }
}
