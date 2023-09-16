using Kitchen;
using KitchenMods;
using Unity.Entities;

namespace KitchenCustomDifficulty
{
    public class ModifyRerollCost : RestaurantSystem
    {
        private static bool _isResetNow = false;

        public struct SResetRerollCostDaily : IComponentData, IModComponent
        {
            public bool IsActive;
            public bool IsResetForDay;
        }

        protected override void Initialise()
        {
            base.Initialise();
        }

        protected override void OnUpdate()
        {
            SResetRerollCostDaily resetCost = GetOrCreate<SResetRerollCostDaily>();

            resetCost.IsActive = Main.PrefSysManager.Get<bool>(Main.SHOP_RESET_REROLL_COST_DAILY_ID);
            if (Has<SIsDayTime>())
            {
                if (!resetCost.IsResetForDay)
                    return;
                resetCost.IsResetForDay = false;
            }
            else
            {
                if (_isResetNow || (resetCost.IsActive && !resetCost.IsResetForDay))
                {
                    _isResetNow = false;
                    if (Require(out SRerollCost rerollCost))
                    {
                        int baseRerollCost = Main.PrefSysManager.Get<int>(Main.SHOP_BASE_REROLL_COST_ID);
                        if (baseRerollCost == -1)
                        {
                            baseRerollCost = Main.DefaultValuesDict[Main.SHOP_BASE_REROLL_COST_ID];
                        }
                        rerollCost.Cost = baseRerollCost;
                        Set(rerollCost);
                        resetCost.IsResetForDay = true;
                    }
                }
            }
            Set(resetCost);
        }

        internal static void ResetNow()
        {
            _isResetNow = true;
        }
    }
}
