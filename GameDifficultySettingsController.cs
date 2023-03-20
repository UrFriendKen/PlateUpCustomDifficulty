using Kitchen;
using KitchenData;

namespace KitchenCustomDifficulty
{
    internal class GameDifficultySettingsController : DaySystem
    {
        private GameDifficultySettings _defaultValues;

        bool _isInit = false;

        protected override void Initialise()
        {
            base.Initialise();
        }

        protected override void OnUpdate()
        {
            if (!_isInit)
            {
                GameDifficultySettings difficulty = GameData.Main.Difficulty;
                _defaultValues = new GameDifficultySettings()
                {
                    CustomersPerHourBase = difficulty.CustomersPerHourBase,
                    CustomersPerHourIncreasePerDay = difficulty.CustomersPerHourIncreasePerDay,
                    CustomerSideChance = difficulty.CustomerSideChance,
                    QueuePatienceTime = difficulty.QueuePatienceTime,
                    QueuePatienceBoost = difficulty.QueuePatienceBoost,
                    CustomerStarterChance = difficulty.CustomerStarterChance,
                    GroupDessertChance = difficulty.GroupDessertChance
                };
                _isInit = true;
            }

            GameData.Main.Difficulty.CustomerStarterChance = _defaultValues.CustomerStarterChance * GetMultiplier(Main.ORDER_STARTER_MODIFIER_ID);
            GameData.Main.Difficulty.CustomerSideChance = _defaultValues.CustomerSideChance * GetMultiplier(Main.ORDER_SIDES_MODIFIER_ID);
            GameData.Main.Difficulty.GroupDessertChance = _defaultValues.GroupDessertChance * GetMultiplier(Main.ORDER_DESSERT_MODIFIER_ID);

            if (Main.PrefSysManager.Get<int>(Main.PHASE_PATIENCE_ENABLED_ID) == 1)
            {
                GameData.Main.Difficulty.QueuePatienceTime = _defaultValues.QueuePatienceTime * GetMultiplier(Main.PATIENCE_QUEUE_ID);
                GameData.Main.Difficulty.QueuePatienceBoost = _defaultValues.QueuePatienceBoost * GetMultiplier(Main.PATIENCE_QUEUE_BOOST_ID);
            }
        }

        private float GetMultiplier(string preferenceID)
        {
            float prefVal = Main.PrefSysManager.Get<int>(preferenceID);
            if (prefVal == -1)
            {
                prefVal = Main.DefaultValuesDict[preferenceID];
            }
            return prefVal / 100f;
        }
    }
}
