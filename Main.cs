using HarmonyLib;
using Kitchen;
using KitchenCustomDifficulty.Preferences;
using KitchenLib;
using KitchenMods;
using System;
using System.Collections.Generic;
using System.Reflection;
using Unity.Entities;
using UnityEngine;

// Namespace should have "Kitchen" in the beginning
namespace KitchenCustomDifficulty
{
    public class Main : BaseMod
    {
        public struct CEndlessModeOfferRestart : IModComponent { }

        // guid must be unique and is recommended to be in reverse domain name notation
        // mod name that is displayed to the player and listed in the mods menu
        // mod version must follow semver e.g. "1.2.3"
        public const string MOD_GUID = "IcedMilo.PlateUp.CustomDifficulty";
        public const string MOD_NAME = "Custom Difficulty";
        public const string MOD_VERSION = "0.4.0";
        public const string MOD_AUTHOR = "IcedMilo";
        public const string MOD_GAMEVERSION = ">=1.1.1";
        // Game version this mod is designed for in semver
        // e.g. ">=1.1.1" current and all future
        // e.g. ">=1.1.1 <=1.2.3" for all from/until

        #region Shop Preferences
        public const string SHOP_TOTAL_APPLIANCE_BLUEPRINTS_ID = "shopApplianceBlueprints";
        public const string SHOP_STAPLE_BLUEPRINTS_ID = "shopStapleBlueprints";
        public const string SHOP_UPGRADED_CHANCE_ID = "shopUpgradedChance";
        #endregion

        #region Restaurant Preferences
        public const string PLAYER_CUSTOMERS_ENABLED_ID = "playerCustomersEnabled";
        public const string BASE_PLAYER_CUSTOMERS_ID = "basePlayerCustomers";
        public const string CUSTOMERS_PER_PLAYER_ID = "playerCustomerMultiplier";

        public const string DAY_LENGTH_ID = "dayLength";

        public const string PLAYER_PATIENCE_ENABLED_ID = "playerPatienceEnabled";
        public const string BASE_PLAYER_PATIENCE_ID = "basePlayerPatienceMultiplier";
        public const string PATIENCE_PER_PLAYER_ID = "playerPatienceMultiplier";

        public const string PHASE_PATIENCE_ENABLED_ID = "phasePatienceEnabled";
        public const string PATIENCE_QUEUE_CAP_ID = "patienceQueueCap";
        public const string PATIENCE_QUEUE_ID = "patienceQueue";
        public const string PATIENCE_QUEUE_BOOST_ID = "patienceQueueBoost";
        public const string PATIENCE_SEATING_ID = "patienceSeating";
        public const string PATIENCE_SERVICE_ID = "patienceService";
        public const string PATIENCE_WAITFORFOOD_ID = "patienceWaitForFood";
        public const string PATIENCE_DELIVERY_ID = "patienceDelivery";
        public const string PATIENCE_DELIVERY_BOOST_ID = "patienceDeliveryBoost";
        #endregion

        #region Order Preferences
        public const string ORDER_THINKING_ID = "orderThinking";
        public const string ORDER_EATING_ID = "orderEating";
        public const string ORDER_STARTER_MODIFIER_ID = "starterModifier";
        public const string ORDER_SIDES_MODIFIER_ID = "sidesModifier";
        public const string ORDER_DESSERT_MODIFIER_ID = "dessertModifier";
        #endregion

        #region Order Cards Preferences
        //public const string CHANGE_MIND_MODIFIER_ID = "changeMindModifier";
        //public const int CHANGE_MIND_MODIFIER_INITIAL = 25;
        //public static readonly MenuPreference ChangeMindModifierPreference = new MenuPreference(CHANGE_MIND_MODIFIER_ID,
        //                                                                                        -2,
        //                                                                                        "Change Mind Chance");


        //public const string REPEAT_COURSE_MODIFIER_ID = "repeatCourseModifier";
        //public const int REPEAT_COURSE_MODIFIER_INITIAL = 25;
        //public static readonly MenuPreference RepeatCourseModifierPreference = new MenuPreference(REPEAT_COURSE_MODIFIER_ID,
        //                                                                                          -2,
        //                                                                                          "Repeat Course Chance");
        #endregion

        #region Player Preferences
        static int PlayersLayer = LayerMask.NameToLayer("Players");
        static int CustomersLayer = LayerMask.NameToLayer("Customers");
        static int DefaultLayer = LayerMask.NameToLayer("Default");
        static int WallsLayer = LayerMask.NameToLayer("Statics");
        static int[] Layers = new int[] { PlayersLayer, CustomersLayer, DefaultLayer, WallsLayer };

        public const string PLAYER_COLLISION_PREP_ID = "playerCollisionPrep";
        public const string PLAYER_COLLISION_ID = "playerCollisionDay";
        public const string PLAYER_OUT_OF_BOUNDS_ID = "playerEnforceBounds";

        public const string PLAYER_SPEED_PREP_ID = "playerSpeedPrep";
        public const string PLAYER_SPEED_ID = "playerSpeed";
        #endregion

        #region Misc Preferences
        public const string FIRE_SPREAD_ID = "fireSpread";

        public const string MESS_FACTOR_ID = "messFactor";

        public const string RESTART_ON_LOSS_ID = "offerRestartOnLoss";
        public const string RESTART_FROM_PREP_END_ID = "restartFromPrepEnd";
        #endregion

        internal static PreferencesManager PrefManager;
        IntArrayGenerator IntArrayGen;
        IntArrayGenerator.IntToStringConversion ValueStringConversion;
        IntArrayGenerator.IntToStringConversion PercentStringConversion;

        public static Dictionary<string, int> DefaultValuesDict;


        public Main() : base(MOD_GUID, MOD_NAME, MOD_AUTHOR, MOD_VERSION, MOD_GAMEVERSION, Assembly.GetExecutingAssembly()) { }

        protected override void OnInitialise()
        {
            base.OnInitialise();
            try
            {
                World.GetExistingSystem(typeof(Kitchen.CheckGameOverFromLife)).Enabled = false;
            }
            catch (NullReferenceException)
            {
                LogInfo("Could not disable system Kitchen.CheckGameOverFromLife!");
            }
            try
            {
                World.GetExistingSystem(typeof(Kitchen.DeterminePlayerSpeed)).Enabled = false;
            }
            catch (NullReferenceException)
            {
                LogInfo("Could not disable system Kitchen.DeterminePlayerSpeed!");
            }
            try
            {
                World.GetExistingSystem(typeof(Kitchen.EnforcePlayerBounds)).Enabled = false;
            }
            catch (NullReferenceException)
            {
                LogInfo("Could not disable system Kitchen.EnforcePlayerBounds!");
            }

            LogWarning($"{MOD_GUID} v{MOD_VERSION} in use!");
        }

        protected override void OnPostActivate(Mod mod)
        {
            InitPreferences();
        }

        private void InitPreferences()
        {
            PrefManager = new PreferencesManager(MOD_GUID, MOD_NAME);
            IntArrayGen = new IntArrayGenerator();
            ValueStringConversion = delegate (string prefKey, int i)
            {
                switch (i)
                {
                    case -2:
                        return "Vanilla";
                    case -1:
                        return $"Default ({DefaultValuesDict[prefKey]})";
                    default:
                        return $"{i}";
                }
            };
            PercentStringConversion = delegate (string prefKey, int i)
            {
                switch (i)
                {
                    case -2:
                        return "Vanilla";
                    case -1:
                        return $"Default ({DefaultValuesDict[prefKey]}%)";
                    default:
                        return $"{i}%";
                }
            };

            DefaultValuesDict = new Dictionary<string, int>()
            {
                { SHOP_TOTAL_APPLIANCE_BLUEPRINTS_ID, 5 },
                { SHOP_STAPLE_BLUEPRINTS_ID, 1 },
                { SHOP_UPGRADED_CHANCE_ID, 30 },
                //{ SHOP_INCREASE_REROLL_COST_ID, 10 },

                { PLAYER_CUSTOMERS_ENABLED_ID, 0 },
                { BASE_PLAYER_CUSTOMERS_ID, 80 },
                { CUSTOMERS_PER_PLAYER_ID, 25 },
                { DAY_LENGTH_ID, 100 },
                { PLAYER_PATIENCE_ENABLED_ID, 0 },
                { BASE_PLAYER_PATIENCE_ID, 75 },
                { PATIENCE_PER_PLAYER_ID, 25 },

                { PHASE_PATIENCE_ENABLED_ID, 0},
                { PATIENCE_QUEUE_CAP_ID, 0 },
                { PATIENCE_QUEUE_ID, 100 },
                { PATIENCE_QUEUE_BOOST_ID, 100 },
                { PATIENCE_SEATING_ID, 100 },
                { PATIENCE_SERVICE_ID, 100 },
                { PATIENCE_WAITFORFOOD_ID, 100},
                { PATIENCE_DELIVERY_ID, 100 },
                { PATIENCE_DELIVERY_BOOST_ID, 100 },

                { ORDER_THINKING_ID, 100 },
                { ORDER_EATING_ID, 100 },
                { ORDER_STARTER_MODIFIER_ID, 100 },
                { ORDER_SIDES_MODIFIER_ID, 100 },
                { ORDER_DESSERT_MODIFIER_ID, 100 },

                //{ CHANGE_MIND_MODIFIER_ID, ChangeMindModifierPreference},
                //{ REPEAT_COURSE_MODIFIER_ID, RepeatCourseModifierPreference },
            
                { PLAYER_COLLISION_PREP_ID, -1 },
                { PLAYER_COLLISION_ID, -1 },
                { PLAYER_OUT_OF_BOUNDS_ID, 0 },
                { PLAYER_SPEED_ID , 100 },
                { PLAYER_SPEED_PREP_ID , 100 },


                { FIRE_SPREAD_ID, 100 },
                { MESS_FACTOR_ID, 100 },
                { RESTART_ON_LOSS_ID, 0 },
                { RESTART_FROM_PREP_END_ID, 1 }
            };

            CreatePreferences();
        }

        private void CreateIntOptionRow(string labelText, string preferenceID, int minInclusive, int maxInclusive, int stepSize, bool giveDisabledOption, bool isPercent, string disabledOptionText = "Vanilla", int? startOptionOverride = null)
        {
            PrefManager.AddLabel(labelText);

            IntArrayGen.Clear();
            if (giveDisabledOption)
            {
                IntArrayGen.Add(-2, disabledOptionText);
            }
            IntArrayGen.Add(-1, $"Default ({DefaultValuesDict[preferenceID]}{(isPercent ? "%" : "")})");
            IntArrayGen.AddRange(minInclusive, maxInclusive, stepSize, preferenceID, isPercent? PercentStringConversion : ValueStringConversion);

            int startingOption = startOptionOverride.HasValue ? startOptionOverride.Value : giveDisabledOption ? -2 : -1;
            PrefManager.AddOption<int>(
                preferenceID,
                labelText,
                startingOption,
                IntArrayGen.GetArray(),
                IntArrayGen.GetStrings());
        }

        private void CreateEnableDisableRow(string labelText, string preferenceID, string enabledText = "Enabled", string disabledText = "Disabled")
        {
            PrefManager.AddLabel(labelText);

            PrefManager.AddOption<int>(
                preferenceID,
                labelText,
                DefaultValuesDict[preferenceID] == 1? 1 : 0,
                new int[] { 0, 1 },
                new string[] { disabledText, enabledText });
        }

        private void CreatePreferences()
        {
            PrefManager.AddLabel("Custom Difficulty");
            PrefManager.AddSpacer();



            #region Shop Settings
            PrefManager.AddSubmenu("Shop", "shop");

            CreateIntOptionRow("Appliance Blueprint Count", SHOP_TOTAL_APPLIANCE_BLUEPRINTS_ID, 0, 20, 1, false, false);
            CreateIntOptionRow("Staple Appliance Blueprint Count", SHOP_STAPLE_BLUEPRINTS_ID, 0, 6, 1, true, false);

            PrefManager.AddSpacer();

            CreateIntOptionRow("Upgraded Chance", SHOP_UPGRADED_CHANCE_ID, 0, 1000, 25, true, true);

            PrefManager.AddSpacer();
            PrefManager.AddSpacer();

            PrefManager.SubmenuDone();
            #endregion


            #region Restaurant Settings
            PrefManager.AddSubmenu("Restaurant", "restaurant");
            PrefManager.AddLabel("Restaurant");
            PrefManager.AddSpacer();

            #region --- Group Count
            PrefManager.AddSubmenu("Group Count", "groupCount");
            CreateEnableDisableRow("Custom Group Count", PLAYER_CUSTOMERS_ENABLED_ID);
            List<int> groupCount_ValList = new List<int>();
            List<string> groupCount_StrList = new List<string>();
            int groupCountStepPercentage1 = 10;
            for (int i = 0; i < (500 / groupCountStepPercentage1); i++)
            {
                int val = i * groupCountStepPercentage1;
                groupCount_ValList.Add(val);
                groupCount_StrList.Add($"{val}%");
            }
            int groupCountStepPercentage2 = 25;
            for (int i = 0; i < (500 / groupCountStepPercentage2); i++)
            {
                int val = 500 + i * groupCountStepPercentage2;
                groupCount_ValList.Add(val);
                groupCount_StrList.Add($"{val}%");
            }
            int groupCountStepPercentage3 = 100;
            for (int i = 0; i < (4000 / groupCountStepPercentage3) + 1; i++)
            {
                int val = 1000 + i * groupCountStepPercentage3;
                groupCount_ValList.Add(val);
                groupCount_StrList.Add($"{val}%");
            }
            PrefManager.AddLabel("Base Group Count");
            PrefManager.AddOption<int>(
                BASE_PLAYER_CUSTOMERS_ID,
                "Base Group Count",
                -1,
                new int[] { -1 }.AddRangeToArray(groupCount_ValList.ToArray()),
                new string[] { $"Default ({DefaultValuesDict[BASE_PLAYER_CUSTOMERS_ID]}%)" }.AddRangeToArray(groupCount_StrList.ToArray()));

            PrefManager.AddLabel("Group Multiplier Per Player");
            PrefManager.AddOption<int>(
                CUSTOMERS_PER_PLAYER_ID,
                "Group Multiplier Per Player",
                -1,
                new int[] { -1 }.AddRangeToArray(groupCount_ValList.ToArray()),
                new string[] { $"Default ({DefaultValuesDict[CUSTOMERS_PER_PLAYER_ID]}%)" }.AddRangeToArray(groupCount_StrList.ToArray()));
            PrefManager.AddSpacer();
            PrefManager.AddSpacer();
            PrefManager.SubmenuDone();
            #endregion

            #region --- Patience
            PrefManager.AddSubmenu("Patience", "patience");
            CreateEnableDisableRow("Custom Player Count Scaling", PLAYER_PATIENCE_ENABLED_ID);
            CreateIntOptionRow("Total Base Patience Decay", BASE_PLAYER_PATIENCE_ID, 0, 500, 10, false, true);
            CreateIntOptionRow("Total Patience Decay Per Player", PATIENCE_PER_PLAYER_ID, 0, 500, 10, false, true);
            PrefManager.AddSpacer();

            #region ------ Phase Tuning
            PrefManager.AddSubmenu("Phase Tuning", "phaseTuning");
            CreateEnableDisableRow("Custom Phase Patience", PHASE_PATIENCE_ENABLED_ID);
            PrefManager.AddSpacer();

            CreateIntOptionRow("Seating Time", PATIENCE_SEATING_ID, 10, 500, 10, false, true);
            CreateIntOptionRow("Service Time", PATIENCE_SERVICE_ID, 10, 500, 10, false, true);
            CreateIntOptionRow("Wait for Food Time", PATIENCE_WAITFORFOOD_ID, 10, 500, 10, false, true);
            CreateIntOptionRow("Delivery Time", PATIENCE_DELIVERY_ID, 10, 500, 10, false, true);
            CreateIntOptionRow("Delivery Recovery", PATIENCE_DELIVERY_BOOST_ID, 0, 500, 10, false, true);

            PrefManager.AddSpacer();    

            #region --------- Queue Phase
            PrefManager.AddSubmenu("Queue", "phaseQueue");
            PrefManager.AddLabel("Queue Settings");
            CreateIntOptionRow("Queue Time", PATIENCE_QUEUE_ID, 10, 500, 10, false, true);
            CreateIntOptionRow("Queue Recovery", PATIENCE_QUEUE_BOOST_ID, 0, 500, 10, false, true);
            PrefManager.AddInfo("PlateUp v1.1.2: \"Queue patience maximum decrease speed has been capped.\" Setting to Uncapped removes this helper.");
            CreateEnableDisableRow("Queue Patience Cap", PATIENCE_QUEUE_CAP_ID, "Uncapped", "Vanilla");
            PrefManager.AddSpacer();
            PrefManager.AddSpacer();
            PrefManager.SubmenuDone();
            #endregion

            PrefManager.AddSpacer();
            PrefManager.AddSpacer();
            PrefManager.SubmenuDone();
            #endregion

            PrefManager.AddSpacer();
            PrefManager.AddSpacer();
            PrefManager.SubmenuDone();
            #endregion

            #region --- Orders
            PrefManager.AddSubmenu("Orders", "orders");
            CreateIntOptionRow("Thinking Time", ORDER_THINKING_ID, 0, 300, 10, false, true);
            CreateIntOptionRow("Eating Time", ORDER_EATING_ID, 0, 300, 10, false, true);
            CreateIntOptionRow("Starter Chance Multiplier", ORDER_STARTER_MODIFIER_ID, 0, 500, 25, false, true);
            CreateIntOptionRow("Sides Chance Multiplier", ORDER_SIDES_MODIFIER_ID, 0, 500, 25, false, true);
            CreateIntOptionRow("Dessert Chance Multiplier", ORDER_DESSERT_MODIFIER_ID, 0, 500, 25, false, true);
            PrefManager.AddSpacer();
            PrefManager.AddSpacer();
            PrefManager.SubmenuDone();
            #endregion

            PrefManager.AddSpacer();

            CreateIntOptionRow("Day Length Multiplier", DAY_LENGTH_ID, 10, 300, 10, true, true);

            PrefManager.AddSpacer();
            PrefManager.AddSpacer();

            PrefManager.SubmenuDone();
            #endregion


            #region Player Settings
            PrefManager.AddSubmenu("Player", "player");

            PrefManager.AddLabel("Prep - Player Collides With");
            PrefManager.AddOption<int>(
                PLAYER_COLLISION_PREP_ID,
                "Prep - Player Collides With",
                DefaultValuesDict[PLAYER_COLLISION_PREP_ID],
                new int[] { -1, 0, 1, 2, 3 },
                new string[] { "Everything", "Everything except players", "Appliances and Walls Only", "Walls Only", "Nothing" });

            PrefManager.AddLabel("Day - Player Collides With");
            PrefManager.AddOption<int>(
                PLAYER_COLLISION_ID,
                "Day - Player Collides With",
                DefaultValuesDict[PLAYER_COLLISION_ID],
                new int[] { -1, 0, 1, 2, 3 },
                new string[] { "Everything", "Everything except players", "Appliances and Walls Only", "Walls Only", "Nothing" });

            PrefManager.AddSpacer();

            CreateEnableDisableRow("Allow Go Out Of Bounds", PLAYER_OUT_OF_BOUNDS_ID);
            PrefManager.AddInfo("Note: The below settings may have undesired behaviour when used with other mods that affect player speed.");
            PrefManager.AddInfo("Set all of them to \"Mod Compatibility\" which allows the other mod to overwrite Custom Difficulty. Use with Caution.");
            CreateIntOptionRow("Prep - Player Speed Modifier", PLAYER_SPEED_PREP_ID, 0, 500, 25, true, true, "Mod Compatibility", startOptionOverride: -1);
            CreateIntOptionRow("Day - Player Speed Modifier", PLAYER_SPEED_ID, 0, 500, 25, true, true, "Mod Compatibility", startOptionOverride: -1);

            PrefManager.AddSpacer();
            PrefManager.AddSpacer();

            PrefManager.SubmenuDone();
            #endregion


            #region Miscellaneous Settings
            PrefManager.AddSubmenu("Misc", "misc");

            List<int> fireSpread_ValList = new List<int>() { -1 };
            List<string> fireSpread_StrList = new List<string>() { $"Default (100%)" };
            int fireSpreadStepPercentage1 = 25;
            for (int i = 0; i < (100 / fireSpreadStepPercentage1); i++)
            {
                int val = i * fireSpreadStepPercentage1;
                fireSpread_ValList.Add(val);
                fireSpread_StrList.Add($"{val}%");
            }
            int fireSpreadStepPercentage2 = 100;
            for (int i = 1; i < (1000 / fireSpreadStepPercentage2); i++)
            {
                int val = i * fireSpreadStepPercentage2;
                fireSpread_ValList.Add(val);
                fireSpread_StrList.Add($"{val}%");
            }
            int fireSpreadStepPercentage3 = 1000;
            for (int i = 1; i < (10000 / fireSpreadStepPercentage3) + 1; i++)
            {
                int val = i * fireSpreadStepPercentage3;
                fireSpread_ValList.Add(val);
                fireSpread_StrList.Add($"{val}%");
            }
            PrefManager.AddLabel("Fire Spread Modifier");
            PrefManager.AddOption<int>(
                FIRE_SPREAD_ID,
                "Fire Spread Modifier",
                -1,
                fireSpread_ValList.ToArray(),
                fireSpread_StrList.ToArray());

            CreateIntOptionRow("Customer Mess Multiplier", MESS_FACTOR_ID, 0, 500, 25, false, true);

            CreateEnableDisableRow("Restart Chance Upon Loss", RESTART_ON_LOSS_ID, "On", "Off");
            CreateEnableDisableRow("Restart From End of Prep Phase", RESTART_FROM_PREP_END_ID);

            PrefManager.AddSpacer();
            PrefManager.AddSpacer();

            PrefManager.SubmenuDone();
            #endregion

            PrefManager.AddSpacer();
            PrefManager.AddSpacer();


            PrefManager.RegisterMenu(PreferencesManager.MenuType.PauseMenu);
        }

        protected override void OnUpdate()
        {
            DoPlayerCollision();
        }

        private void DoPlayerCollision()
        {
            int ignoreCollisionThreshold;
            if (GameInfo.IsPreparationTime)
            {
                ignoreCollisionThreshold = PrefManager.Get<int>(PLAYER_COLLISION_PREP_ID);
            }
            else
            {
                ignoreCollisionThreshold = PrefManager.Get<int>(PLAYER_COLLISION_ID);
            }
            for (int i = 0; i < Layers.Length; i++)
            {
                bool ignoreCollision = i <= ignoreCollisionThreshold;
                Physics.IgnoreLayerCollision(PlayersLayer, Layers[i], ignore: ignoreCollision);
            }
        }

        #region Logging
        // You can remove this, I just prefer a more standardized logging
        public static void LogInfo(string _log) { Debug.Log($"[{MOD_NAME}] " + _log); }
        public static void LogWarning(string _log) { Debug.LogWarning($"[{MOD_NAME}] " + _log); }
        public static void LogError(string _log) { Debug.LogError($"[{MOD_NAME}] " + _log); }
        public static void LogInfo(object _log) { LogInfo(_log.ToString()); }
        public static void LogWarning(object _log) { LogWarning(_log.ToString()); }
        public static void LogError(object _log) { LogError(_log.ToString()); }
        #endregion
    }
}
