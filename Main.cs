using HarmonyLib;
using Kitchen;
using KitchenLib;
using KitchenMods;
using PreferenceSystem;
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
        // guid must be unique and is recommended to be in reverse domain name notation
        // mod name that is displayed to the player and listed in the mods menu
        // mod version must follow semver e.g. "1.2.3"
        public const string MOD_GUID = "IcedMilo.PlateUp.CustomDifficulty";
        public const string MOD_NAME = "Custom Difficulty";
        public const string MOD_VERSION = "1.0.4";
        public const string MOD_AUTHOR = "IcedMilo";
        public const string MOD_GAMEVERSION = ">=1.1.1";
        // Game version this mod is designed for in semver
        // e.g. ">=1.1.1" current and all future
        // e.g. ">=1.1.1 <=1.2.3" for all from/until

        #region Shop Preferences
        public const string SHOP_TOTAL_APPLIANCE_BLUEPRINTS_ID = "shopApplianceBlueprints";
        public const string SHOP_STAPLE_BLUEPRINTS_ID = "shopStapleBlueprints";
        public const string SHOP_COST_MULTIPLIER = "shopCostMultiplier";
        public const string SHOP_UPGRADED_CHANCE_ID = "shopUpgradedChance";
        public const string DESK_AUTO_RESEARCH_ID = "deskAutoResearch";
        public const string DESK_AUTO_COPY_ID = "deskAutoCopy";
        public const string DESK_AUTO_MAKE_FREE_ID = "deskAutoMakeFree";
        public const string SHOP_BASE_REROLL_COST_ID = "shopBaseRerollCost";
        public const string SHOP_REROLL_COST_INCREASE_ID = "shopRerollCostIncrease";
        public const string SHOP_RESET_REROLL_COST_DAILY_ID = "shopResetRerollCostDaily";
        #endregion

        #region Restaurant Preferences
        public const string PLAYER_CUSTOMERS_ENABLED_ID = "playerCustomersEnabled";
        public const string BASE_PLAYER_CUSTOMERS_ID = "basePlayerCustomers";
        public const string CUSTOMERS_PER_PLAYER_ID = "playerCustomerMultiplier";
        public const string CUSTOMERS_MIN_GROUP_SIZE_ID = "minGroupSize";
        public const string CUSTOMERS_MAX_GROUP_SIZE_ID = "maxGroupSize";
        public const string CARD_CUSTOMER_CHANGE_PER_POINT_ID = "cardCustomerChangePerPoint";

        public const string DAY_LENGTH_ID = "dayLength";
        public const string WEATHER_ACTIVE_ID = "weatherActive";

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

        public const string DECORATION_EXCLUSIVE_ID = "decorationExclusive";
        public const string DECORATION_AFFORDABLE_ID = "decorationAffordable";
        public const string DECORATION_CHARMING_ID = "decorationCharming";
        public const string DECORATION_FORMAL_ID = "decorationFormal";
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
        public const string FIRE_SPREAD_THROUGH_WALLS_ID = "fireSpreadThroughWalls";

        public const string MESS_FACTOR_ID = "messFactor";
        //public const string MESS_REDUCES_NEARBY_PATIENCE_ID = "messReducesPatience";

        public const string CUSTOMER_SPEED_ID = "customerSpeed";

        public const string RESTART_ON_LOSS_ID = "offerRestartOnLoss";
        #endregion

        public static Dictionary<string, int> DefaultValuesDict;


        internal static PreferenceSystemManager PrefSysManager;


        public Main() : base(MOD_GUID, MOD_NAME, MOD_AUTHOR, MOD_VERSION, MOD_GAMEVERSION, Assembly.GetExecutingAssembly()) { }

        protected override void OnInitialise()
        {
            base.OnInitialise();
            LogWarning($"{MOD_GUID} v{MOD_VERSION} in use!");
            TrySetSystemEnabled<Kitchen.CheckGameOverFromLife>(false);
        }

        protected override void OnUpdate()
        {
        }

        private bool TrySetSystemEnabled<T>(bool isEnabled, bool logError = true) where T : GenericSystemBase
        {
            try
            {
                World.GetExistingSystem(typeof(T)).Enabled = isEnabled;
                LogInfo($"{(isEnabled ? "Enabled" : "Disabled")} {typeof(T).FullName} system.");
                return true;
            }
            catch (NullReferenceException)
            {
                LogInfo($"Failed to {(isEnabled ? "enable" : "disable")} {typeof(T).FullName} system. Are you in Multiplayer?");
                return false;
            }
        }

        protected override void OnPostActivate(Mod mod)
        {
            //InitPreferences();
            CreatePreferencesNew();

            //Events.BuildGameDataEvent += delegate (object _, BuildGameDataEventArgs args)
            //{
            //    Dictionary<int, float> messEffectPatienceModifierValues = new Dictionary<int, float>()
            //    {
            //        { ApplianceReferences.MessCustomer1, -0.05f },
            //        { ApplianceReferences.MessCustomer2, -0.1f },
            //        { ApplianceReferences.MessCustomer3, -0.15f },
            //        { ApplianceReferences.MessKitchen1, -0.05f },
            //        { ApplianceReferences.MessKitchen2, -0.1f },
            //        { ApplianceReferences.MessKitchen3, -0.15f }
            //    };

            //    foreach (KeyValuePair<int, float> item in messEffectPatienceModifierValues)
            //    {
            //        if (!args.gamedata.TryGet(item.Key, out Appliance mess, warn_if_fail: true))
            //            continue;

            //        mess.EffectCondition = new CEffectPreferenceCondition(PrefSysManager, MESS_REDUCES_NEARBY_PATIENCE_ID)
            //        {
            //            ConditionWhenEnabled = CEffectPreferenceCondition.Condition.Always,
            //            ConditionWhenDisabled = CEffectPreferenceCondition.Condition.Never
            //        };
            //        mess.EffectType = new CTableModifier() { PatienceModifiers = GetMessPatienceModifier(item.Value) };
            //        mess.EffectRange = new CEffectRangeTiles()
            //        {
            //            Tiles = 2,
            //            PassThroughWalls = false
            //        };
            //    }
            //};

            //PatienceValues GetMessPatienceModifier(float modifier)
            //{
            //    modifier = Mathf.Clamp(modifier, -1, 1);
            //    return new PatienceValues()
            //    {
            //        Seating = modifier,
            //        Service = modifier,
            //        WaitForFood = modifier,
            //        GetFoodDelivered = modifier
            //    };
            //}
        }

        private void CreatePreferencesNew()
        {
            DefaultValuesDict = new Dictionary<string, int>()
            {
                { SHOP_TOTAL_APPLIANCE_BLUEPRINTS_ID, 5 },
                { SHOP_STAPLE_BLUEPRINTS_ID, 1 },
                { SHOP_COST_MULTIPLIER, 100 },
                { SHOP_UPGRADED_CHANCE_ID, 30 },
                { DESK_AUTO_RESEARCH_ID, 0 },
                { DESK_AUTO_COPY_ID, 0 },
                { DESK_AUTO_MAKE_FREE_ID, 0 },
                { SHOP_BASE_REROLL_COST_ID, 10 },
                { SHOP_REROLL_COST_INCREASE_ID, 10 },

                { PLAYER_CUSTOMERS_ENABLED_ID, 0 },
                { BASE_PLAYER_CUSTOMERS_ID, 80 },
                { CUSTOMERS_PER_PLAYER_ID, 25 },
                { CUSTOMERS_MIN_GROUP_SIZE_ID, 1 },
                { CUSTOMERS_MAX_GROUP_SIZE_ID, 2 },
                { CARD_CUSTOMER_CHANGE_PER_POINT_ID, 15 },
                { DAY_LENGTH_ID, 100 },
                { WEATHER_ACTIVE_ID, -2 },
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

                { DECORATION_EXCLUSIVE_ID, 0 },
                { DECORATION_AFFORDABLE_ID, 0 },
                { DECORATION_CHARMING_ID, 0 },
                { DECORATION_FORMAL_ID, 0 },

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
                { FIRE_SPREAD_THROUGH_WALLS_ID, 0 },
                { MESS_FACTOR_ID, 100 },
                { CUSTOMER_SPEED_ID, 100 },
                { RESTART_ON_LOSS_ID, 0 }
            };

            PrefSysManager = new PreferenceSystemManager(MOD_GUID, MOD_NAME);
            string[] strings;
            PrefSysManager
                .AddLabel("Custom Difficulty")
                .AddSpacer()
                .AddLabel("Selected Profile")
                .AddProfileSelector()
                .AddDeleteProfileButton("Delete Profile")
                .AddSpacer()
            #region Shop
                .AddSubmenu("Shop", "shop")
                    .AddSubmenu("Blueprints", "shop_blueprints")
                        .AddLabel("Total Appliance Blueprint Count")
                        .AddOption<int>(
                            SHOP_TOTAL_APPLIANCE_BLUEPRINTS_ID,
                            -1,
                            GenerateIntArray("1|20", out strings, addValuesBefore: new int[] { -1 }),
                            new string[] { $"Default ({DefaultValuesDict[SHOP_TOTAL_APPLIANCE_BLUEPRINTS_ID]})" }.AddRangeToArray(strings))
                        .AddLabel("Staple Appliance Blueprint Count")
                        .AddOption<int>(
                            SHOP_STAPLE_BLUEPRINTS_ID,
                            -2,
                            GenerateIntArray("0|5", out strings, addValuesBefore: new int[] { -2, -1 }),
                            new string[] { "Vanilla", $"Default ({DefaultValuesDict[SHOP_STAPLE_BLUEPRINTS_ID]})" }.AddRangeToArray(strings))
                        .AddLabel("Blueprint Cost Multiplier")
                        .AddOption<int>(
                            SHOP_COST_MULTIPLIER,
                            -1,
                            GenerateIntArray("0|100|5", out strings, addValuesBefore: new int[] { -1 }, postfix: "%"),
                            new string[] { $"Default ({DefaultValuesDict[SHOP_COST_MULTIPLIER]})" }.AddRangeToArray(strings))
                        .AddSpacer()
                        .AddLabel("Upgraded Chance")
                        .AddOption<int>(
                            SHOP_UPGRADED_CHANCE_ID,
                            -2,
                            GenerateIntArray("0|1000|25", out strings, addValuesBefore: new int[] { -2, -1 }, postfix: "%"),
                            new string[] { "Vanilla", $"Default ({DefaultValuesDict[SHOP_UPGRADED_CHANCE_ID]}%)" }.AddRangeToArray(strings))
                        .AddSpacer()
                        .AddSpacer()
                    .SubmenuDone()
                    .AddSubmenu("Desks", "shop_desks")
                        .AddLabel("Automatically Research")
                        .AddOption<int>(
                            DESK_AUTO_RESEARCH_ID,
                            0,
                            new int[] { 0, 1 },
                            new string[] { "Disabled", "Enabled" })
                        .AddLabel("Automatically Copy")
                        .AddOption<int>(
                            DESK_AUTO_COPY_ID,
                            0,
                            new int[] { 0, 1 },
                            new string[] { "Disabled", "Enabled" })
                        .AddLabel("Automatically Discount")
                        .AddOption<int>(
                            DESK_AUTO_MAKE_FREE_ID,
                            0,
                            new int[] { 0, 1 },
                            new string[] { "Disabled", "Enabled" })
                        .AddSpacer()
                        .AddSpacer()
                    .SubmenuDone()
                    .AddSubmenu("Reroll", "shop_reroll")
                        .AddLabel("Base Reroll Cost")
                        .AddOption<int>(
                            SHOP_BASE_REROLL_COST_ID,
                            -1,
                            GenerateIntArray("0|1000|10", out strings, addValuesBefore: new int[] { -1 }),
                            new string[] { $"Default ({DefaultValuesDict[SHOP_BASE_REROLL_COST_ID]})" }.AddRangeToArray(strings))
                        //.AddLabel("Reroll Cost Increase")
                        //.AddOption<int>(
                        //    SHOP_REROLL_COST_INCREASE_ID,
                        //    -1,
                        //    GenerateIntArray("0|1000|10", out strings, addValuesBefore: new int[] { -1 }),
                        //    new string[] { $"Default ({DefaultValuesDict[SHOP_REROLL_COST_INCREASE_ID]})" }.AddRangeToArray(strings))
                        .AddLabel("Reset Reroll Cost")
                        .AddOption<bool>(
                            SHOP_RESET_REROLL_COST_DAILY_ID,
                            false,
                            new bool[] { false, true },
                            new string[] { "Never", "Everyday" })
                        .AddSpacer()
                        .AddButton("Reset Reroll Cost Now",
                            delegate(int _)
                            {
                                ModifyRerollCost.ResetNow();
                            })
                        .AddSpacer()
                        .AddSpacer()
                    .SubmenuDone()
                .AddSpacer()
                .AddSpacer()
                .SubmenuDone()
            #endregion
            #region Restaurant
                .AddSubmenu("Restaurant", "restaurant")
                    .AddLabel("Restaurant")
                    .AddSpacer()
            #region Group Count
                    .AddSubmenu("Group Count", "restaurant_groupCount")
                        .AddLabel("Custom Group Count")
                        .AddOption<int>(
                            PLAYER_CUSTOMERS_ENABLED_ID,
                            0,
                            new int[] { 0, 1 },
                            new string[] { "Disabled", "Enabled" })
                        .AddLabel("Base Group Count")
                        .AddOption<int>(
                            BASE_PLAYER_CUSTOMERS_ID,
                            -1,
                            GenerateIntArray("0|490|10,500|975|25,1000|5000|100", out strings, addValuesBefore: new int[] { -1 }, postfix: "%"),
                            new string[] { $"Default ({DefaultValuesDict[BASE_PLAYER_CUSTOMERS_ID]}%)" }.AddRangeToArray(strings))
                        .AddLabel("Group Multiplier Per Player")
                        .AddOption<int>(
                            CUSTOMERS_PER_PLAYER_ID,
                            -1,
                            GenerateIntArray("0|490|10,500|975|25,1000|5000|100", out strings, addValuesBefore: new int[] { -1 }, postfix: "%"),
                            new string[] { $"Default ({DefaultValuesDict[BASE_PLAYER_CUSTOMERS_ID]}%)" }.AddRangeToArray(strings))
                        .AddSpacer()
                        .AddLabel("Min Group Size")
                        .AddOption<int>(
                            CUSTOMERS_MIN_GROUP_SIZE_ID,
                            -2,
                            GenerateIntArray("1|100", out strings, addValuesBefore: new int[] { -2, -1 }),
                            new string[] { "Vanilla", $"Default ({DefaultValuesDict[CUSTOMERS_MIN_GROUP_SIZE_ID]})" }.AddRangeToArray(strings))
                        .AddLabel("Max Group Size")
                        .AddOption<int>(
                            CUSTOMERS_MAX_GROUP_SIZE_ID,
                            -2,
                            GenerateIntArray("1|100", out strings, addValuesBefore: new int[] { -2, -1 }),
                            new string[] { "Vanilla", $"Default ({DefaultValuesDict[CUSTOMERS_MAX_GROUP_SIZE_ID]})" }.AddRangeToArray(strings))
                        .AddLabel("Card Customer Change Per Point")
                        .AddOption<int>(
                            CARD_CUSTOMER_CHANGE_PER_POINT_ID,
                            -1,
                            GenerateIntArray("-100|100|5", out strings, addValuesBefore: new int[] { -1 }, postfix: "%"),
                            new string[] { $"Default ({DefaultValuesDict[CARD_CUSTOMER_CHANGE_PER_POINT_ID]}%)" }.AddRangeToArray(strings))
                        .AddSpacer()
                        .AddSpacer()
                    .SubmenuDone()
            #endregion
            #region Patience
                    .AddSubmenu("Patience", "restaurant_patience")
                        .AddLabel("Patience")
                        .AddSpacer()
                        .AddLabel("Custom Player Count Scaling")
                        .AddOption<int>(
                            PLAYER_PATIENCE_ENABLED_ID,
                            0,
                            new int[] { 0, 1 },
                            new string[] { "Disabled", "Enabled" })
                        .AddLabel("Total Base Patience Decay")
                        .AddOption<int>(
                            BASE_PLAYER_PATIENCE_ID,
                            -1,
                            GenerateIntArray("0|500|10", out strings, addValuesBefore: new int[] { -1 }, postfix: "%"),
                            new string[] { $"Default ({DefaultValuesDict[BASE_PLAYER_PATIENCE_ID]}%)" }.AddRangeToArray(strings))
                        .AddLabel("Group Multiplier Per Player")
                        .AddOption<int>(
                            PATIENCE_PER_PLAYER_ID,
                            -1,
                            GenerateIntArray("0|500|10", out strings, addValuesBefore: new int[] { -1 }, postfix: "%"),
                            new string[] { $"Default ({DefaultValuesDict[PATIENCE_PER_PLAYER_ID]}%)" }.AddRangeToArray(strings))
                        .AddSpacer()
            #region Phase Tuning
                        .AddSubmenu("Phase Tuning", "restaurant_patience_phaseTuning")
                            .AddLabel("Custom Phase Patience")
                            .AddOption<int>(
                                PHASE_PATIENCE_ENABLED_ID,
                                0,
                                new int[] { 0, 1 },
                                new string[] { "Disabled", "Enabled" })
                            .AddSpacer()
                            .AddLabel("Seating Time")
                            .AddOption<int>(
                                PATIENCE_SEATING_ID,
                                -1,
                                GenerateIntArray("0|500|10", out strings, addValuesBefore: new int[] { -1 }, postfix: "%"),
                                new string[] { $"Default ({DefaultValuesDict[PATIENCE_SEATING_ID]}%)" }.AddRangeToArray(strings))
                            .AddLabel("Service Time")
                            .AddOption<int>(
                                PATIENCE_SERVICE_ID,
                                -1,
                                GenerateIntArray("0|500|10", out strings, addValuesBefore: new int[] { -1 }, postfix: "%"),
                                new string[] { $"Default ({DefaultValuesDict[PATIENCE_SERVICE_ID]}%)" }.AddRangeToArray(strings))
                            .AddLabel("Wait For Food Time")
                            .AddOption<int>(
                                PATIENCE_WAITFORFOOD_ID,
                                -1,
                                GenerateIntArray("0|500|10", out strings, addValuesBefore: new int[] { -1 }, postfix: "%"),
                                new string[] { $"Default ({DefaultValuesDict[PATIENCE_WAITFORFOOD_ID]}%)" }.AddRangeToArray(strings))
                            .AddLabel("Delivery Time")
                            .AddOption<int>(
                                PATIENCE_DELIVERY_ID,
                                -1,
                                GenerateIntArray("0|500|10", out strings, addValuesBefore: new int[] { -1 }, postfix: "%"),
                                new string[] { $"Default ({DefaultValuesDict[PATIENCE_DELIVERY_ID]}%)" }.AddRangeToArray(strings))
                            .AddLabel("Delivery Recovery")
                            .AddOption<int>(
                                PATIENCE_DELIVERY_BOOST_ID,
                                -1,
                                GenerateIntArray("0|500|10", out strings, addValuesBefore: new int[] { -1 }, postfix: "%"),
                                new string[] { $"Default ({DefaultValuesDict[PATIENCE_DELIVERY_BOOST_ID]}%)" }.AddRangeToArray(strings))
                            .AddSpacer()
            #region Queue
                            .AddSubmenu("Queue", "restaurant_patience_phaseTuning_queue")
                                .AddLabel("Queue Time")
                                .AddOption<int>(
                                    PATIENCE_QUEUE_ID,
                                    -1,
                                    GenerateIntArray("0|500|10", out strings, addValuesBefore: new int[] { -1 }, postfix: "%"),
                                    new string[] { $"Default ({DefaultValuesDict[PATIENCE_QUEUE_ID]}%)" }.AddRangeToArray(strings))
                                .AddLabel("Queue Recovery")
                                .AddOption<int>(
                                    PATIENCE_QUEUE_BOOST_ID,
                                    -1,
                                    GenerateIntArray("0|500|10", out strings, addValuesBefore: new int[] { -1 }, postfix: "%"),
                                    new string[] { $"Default ({DefaultValuesDict[PATIENCE_QUEUE_BOOST_ID]}%)" }.AddRangeToArray(strings))
                                .AddInfo("PlateUp v1.1.2: \"Queue patience maximum decrease speed has been capped.\" Setting to Uncapped removes this helper.")
                                .AddLabel("Queue Patience Cap")
                                .AddOption<int>(
                                    PATIENCE_QUEUE_CAP_ID,
                                    0,
                                    new int[] { 0, 1 },
                                    new string[] { "Vanilla", "Uncapped" })
                                .AddSpacer()
                                .AddSpacer()
                            .SubmenuDone()
            #endregion
                            .AddSpacer()
                            .AddSpacer()
                        .SubmenuDone()
            #endregion
                        .AddSpacer()
                        .AddSpacer()
                    .SubmenuDone()
            #endregion
            #region Orders
                    .AddSubmenu("Orders", "restaurant_orders")
                        .AddLabel("Orders")
                        .AddSpacer()
                        .AddLabel("Thinking Time")
                        .AddOption<int>(
                            ORDER_THINKING_ID,
                            -1,
                            GenerateIntArray("0|300|10", out strings, addValuesBefore: new int[] { -1 }, postfix: "%"),
                            new string[] { $"Default ({DefaultValuesDict[ORDER_THINKING_ID]}%)" }.AddRangeToArray(strings))
                        .AddLabel("Eating Time")
                        .AddOption<int>(
                            ORDER_EATING_ID,
                            -1,
                            GenerateIntArray("0|300|10", out strings, addValuesBefore: new int[] { -1 }, postfix: "%"),
                            new string[] { $"Default ({DefaultValuesDict[ORDER_EATING_ID]}%)" }.AddRangeToArray(strings))
                        .AddSpacer()
                        .AddLabel("Starter Chance Multiplier")
                        .AddOption<int>(
                            ORDER_STARTER_MODIFIER_ID,
                            -1,
                            GenerateIntArray("0|500|25", out strings, addValuesBefore: new int[] { -1 }, postfix: "%"),
                            new string[] { $"Default ({DefaultValuesDict[ORDER_STARTER_MODIFIER_ID]}%)" }.AddRangeToArray(strings))
                        .AddLabel("Sides Chance Multiplier")
                        .AddOption<int>(
                            ORDER_SIDES_MODIFIER_ID,
                            -1,
                            GenerateIntArray("0|500|25", out strings, addValuesBefore: new int[] { -1 }, postfix: "%"),
                            new string[] { $"Default ({DefaultValuesDict[ORDER_SIDES_MODIFIER_ID]}%)" }.AddRangeToArray(strings))
                        .AddLabel("Dessert Chance Multiplier")
                        .AddOption<int>(
                            ORDER_DESSERT_MODIFIER_ID,
                            -1,
                            GenerateIntArray("0|500|25", out strings, addValuesBefore: new int[] { -1 }, postfix: "%"),
                            new string[] { $"Default ({DefaultValuesDict[ORDER_DESSERT_MODIFIER_ID]}%)" }.AddRangeToArray(strings))
                        .AddSpacer()
                        .AddSpacer()
                    .SubmenuDone()
            #endregion
                    .AddSpacer()
                    .AddLabel("Day Length Multiplier")
                    .AddOption<int>(
                        DAY_LENGTH_ID,
                        -2,
                        GenerateIntArray("0|300|10", out strings, addValuesBefore: new int[] { -2, -1 }, postfix: "%"),
                        new string[] { "Vanilla", $"Default ({DefaultValuesDict[DAY_LENGTH_ID]}%)" }.AddRangeToArray(strings))
                    .AddLabel("Weather Active (Start of Day)")
                    .AddOption<int>(
                        WEATHER_ACTIVE_ID,
                        -2,
                        new int[] { -2, 0, 1 },
                        new string[] { "Vanilla", "Disabled", "Enabled" })
                    .AddSpacer()
                    .AddSpacer()
                .SubmenuDone()
            #endregion
            #region Player
                .AddSubmenu("Player", "player")
                    .AddLabel("Prep - Player Collides With")
                    .AddOption<int>(
                        PLAYER_COLLISION_PREP_ID,
                        DefaultValuesDict[PLAYER_COLLISION_PREP_ID],
                        new int[] { -1, 0, 1, 2, 3 },
                        new string[] { "Everything", "Everything except players", "Appliances and Walls Only", "Walls Only", "Nothing" })
                    .AddLabel("Day - Player Collides With")
                    .AddOption<int>(
                        PLAYER_COLLISION_ID,
                        DefaultValuesDict[PLAYER_COLLISION_ID],
                        new int[] { -1, 0, 1, 2, 3 },
                        new string[] { "Everything", "Everything except players", "Appliances and Walls Only", "Walls Only", "Nothing" })
                    .AddLabel("Allow Go Out of Bounds")
                    .AddOption<int>(
                        PLAYER_OUT_OF_BOUNDS_ID,
                        0,
                        new int[] { 0, 1 },
                        new string[] { "Disabled", "Enabled" })
                    .AddSpacer()
                    .AddInfo("Note: The below settings may have undesired behaviour when used with other mods that affect player speed.")
                    .AddInfo("Set all of them to \"Mod Compatibility\" which allows the other mod to overwrite Custom Difficulty. Use with Caution.")
                    .AddLabel("Prep - Player Speed Modifier")
                    .AddOption<int>(
                        PLAYER_SPEED_PREP_ID,
                        -1,
                        GenerateIntArray("0|500|25", out strings, addValuesBefore: new int[] { -2, -1 }, postfix: "%"),
                        new string[] { "Mod Compatibility", $"Default ({DefaultValuesDict[PLAYER_SPEED_PREP_ID]}%)" }.AddRangeToArray(strings))
                    .AddLabel("Day - Player Speed Modifier")
                    .AddOption<int>(
                        PLAYER_SPEED_ID,
                        -1,
                        GenerateIntArray("0|500|25", out strings, addValuesBefore: new int[] { -2, -1 }, postfix: "%"),
                        new string[] { "Mod Compatibility", $"Default ({DefaultValuesDict[PLAYER_SPEED_ID]}%)" }.AddRangeToArray(strings))
                    .AddSpacer()
                    .AddSpacer()
                .SubmenuDone()
            #endregion
            #region Misc
                .AddSubmenu("Misc", "misc")
                    .AddLabel("Fire Spread Modifier")
                    .AddOption<int>(
                        FIRE_SPREAD_ID,
                        -1,
                        GenerateIntArray("0|75|25,100|900|100,1000|10000|1000", out strings, addValuesBefore: new int[] { -1 }, postfix: "%"),
                        new string[] { $"Default ({DefaultValuesDict[FIRE_SPREAD_ID]}%)" }.AddRangeToArray(strings))
                    .AddLabel("Fire Spread Through Walls")
                    .AddOption<int>(
                        FIRE_SPREAD_THROUGH_WALLS_ID,
                        0,
                        new int[] { 0, 1 },
                        new string[] { "Disabled", "Enabled" })
                    .AddLabel("Customer Mess Multiplier")
                    .AddOption<int>(
                        MESS_FACTOR_ID,
                        -1,
                        GenerateIntArray("0|500|25", out strings, addValuesBefore: new int[] { -1 }, postfix: "%"),
                        new string[] { $"Default ({DefaultValuesDict[MESS_FACTOR_ID]}%)" }.AddRangeToArray(strings))
                    //.AddLabel("Mess Reduces Patience")
                    //.AddInfo("Decreases 5% per level of mess nearby.")
                    //.AddOption<bool>(
                    //    MESS_REDUCES_NEARBY_PATIENCE_ID,
                    //    false,
                    //    new bool[] { false, true },
                    //    new string[] { "Disabled", "Enabled" })
                    .AddLabel("Customer Walk Speed")
                    .AddOption<int>(
                        CUSTOMER_SPEED_ID,
                        -1,
                        GenerateIntArray("0|500|25", out strings, addValuesBefore: new int[] { -1 }, postfix: "%"),
                        new string[] { $"Default ({DefaultValuesDict[CUSTOMER_SPEED_ID]}%)" }.AddRangeToArray(strings))
                    .AddLabel("Restart Chance Upon Loss")
                    .AddOption<int>(
                        RESTART_ON_LOSS_ID,
                        0,
                        new int[] { 0, 1 },
                        new string[] { "Off", "On" })
                    .AddSpacer()
                    .AddSpacer()
                .SubmenuDone()
            #endregion
                .AddSpacer()
                .AddSpacer();

            PrefSysManager.RegisterMenu(PreferenceSystemManager.MenuType.PauseMenu);
        }

        private int[] GenerateIntArray(string input, out string[] stringRepresentation, int[] addValuesBefore = null, int[] addValuesAfter = null, string prefix = "", string postfix = "")
        {
            List<string> stringOutput = new List<string>();
            List<int> output = new List<int>();
            string[] ranges = input.Split(',');
            foreach (string range in ranges)
            {
                string[] extents = range.Split('|');
                int min = Convert.ToInt32(extents[0]);
                int max;
                int step;
                switch (extents.Length)
                {
                    case 1:
                        output.Add(min);
                        stringOutput.Add($"{prefix}{min}{postfix}");
                        continue;
                    case 2:
                        max = Convert.ToInt32(extents[1]);
                        step = 1;
                        break;
                    case 3:
                        max = Convert.ToInt32(extents[1]);
                        step = Convert.ToInt32(extents[2]);
                        break;
                    default:
                        continue;
                }
                for (int i = min; i <= max; i += step)
                {
                    output.Add(i);
                    stringOutput.Add($"{prefix}{i}{postfix}");
                }
            }
            stringRepresentation = stringOutput.ToArray();
            if (addValuesBefore == null)
                addValuesBefore = new int[0];
            if (addValuesAfter == null)
                addValuesAfter = new int[0];
            return addValuesBefore.AddRangeToArray(output.ToArray()).AddRangeToArray(addValuesAfter);
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
