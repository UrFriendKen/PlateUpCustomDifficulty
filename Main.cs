using Kitchen;
using KitchenCustomDifficulty.Preferences;
using KitchenLib;
using KitchenLib.Event;
using KitchenLib.Utils;
using KitchenMods;
using System;
using System.Collections.Generic;
using System.Reflection;
using Unity.Entities;
using UnityEngine;

//using KitchenCustomDifficulty.Views;


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
        public const string MOD_VERSION = "0.3.8";
        public const string MOD_AUTHOR = "IcedMilo";
        public const string MOD_GAMEVERSION = ">=1.1.1";
        // Game version this mod is designed for in semver
        // e.g. ">=1.1.1" current and all future
        // e.g. ">=1.1.1 <=1.2.3" for all from/until

        #region Shop Preferences
        public const string SHOP_TOTAL_APPLIANCE_BLUEPRINTS_ID = "shopApplianceBlueprints";
        //public const int SHOP_TOTAL_APPLIANCE_BLUEPRINTS_INITIAL = 5;
        //public static readonly MenuPreference ShopTotalApplianceBlueprintsPreference = new MenuPreference(SHOP_TOTAL_APPLIANCE_BLUEPRINTS_ID,
        //                                                                                                  -1,
        //                                                                                                  "Appliance Blueprint Count");

        public const string SHOP_STAPLE_BLUEPRINTS_ID = "shopStapleBlueprints";
        //public const int SHOP_STAPLE_BLUEPRINTS_INITIAL = 1;
        //public static readonly MenuPreference ShopStapleBlueprintsPreference = new MenuPreference(SHOP_STAPLE_BLUEPRINTS_ID,
        //                                                                                          -2,
        //                                                                                          "Staple Appliance Blueprint Count");
        
        public const string SHOP_UPGRADED_CHANCE_ID = "shopUpgradedChance";
        //public const int SHOP_UPGRADED_CHANCE_INITIAL = 30;
        //public static readonly MenuPreference ShopUpgradedChancePreference = new MenuPreference(SHOP_UPGRADED_CHANCE_ID,
        //                                                                                        -2,
        //                                                                                        "Upgraded Chance");
        /*
        public const string SHOP_INCREASE_REROLL_COST_ID = "shopIncreaseRerollCost";
        public const int SHOP_INCREASE_REROLL_COST_INITIAL = 10;
        public static readonly MenuPreference ShopIncreaseRerollCostPreference = new MenuPreference(SHOP_INCREASE_REROLL_COST_ID,
                                                                                                    -1,
                                                                                                    "Reroll Cost Increase");
        */
        #endregion

        #region Customer Preferences
        public const string PLAYER_CUSTOMERS_ENABLED_ID = "playerCustomersEnabled";
        //public const int PLAYER_CUSTOMERS_ENABLED_INITIAL = 0;
        //public static readonly MenuPreference PlayerCustomersEnabledPreference = new MenuPreference(PLAYER_CUSTOMERS_ENABLED_ID,
        //                                                                                            0,
        //                                                                                            "Custom Customer Count");

        public const string BASE_PLAYER_CUSTOMERS_ID = "basePlayerCustomers";
        //public const int BASE_PLAYER_CUSTOMERS_INITIAL = 80;
        //public static readonly MenuPreference BasePlayerCustomersPreference = new MenuPreference(BASE_PLAYER_CUSTOMERS_ID,
        //                                                                                        -1,
        //                                                                                        "Base Customer Count");

        public const string CUSTOMERS_PER_PLAYER_ID = "playerCustomerMultiplier";
        //public const int CUSTOMERS_PER_PLAYER_INITIAL = 25;
        //public static readonly MenuPreference CustomersPerPlayerPreference = new MenuPreference(CUSTOMERS_PER_PLAYER_ID,
        //                                                                                        -2,
        //                                                                                        "Customers Multiplier Per Player");

        public const string PLAYER_PATIENCE_ENABLED_ID = "playerPatienceEnabled";
        //public const int PLAYER_PATIENCE_ENABLED_INITIAL = 0;
        //public static readonly MenuPreference PlayerPatienceEnabledPreference = new MenuPreference(PLAYER_PATIENCE_ENABLED_ID,
        //                                                                                           0,
        //                                                                                           "Custom Patience");

        public const string BASE_PLAYER_PATIENCE_ID = "basePlayerPatienceMultiplier";
        //public const int BASE_PLAYER_PATIENCE_INITIAL = 75;
        //public static readonly MenuPreference BasePlayerPatiencePreference = new MenuPreference(BASE_PLAYER_PATIENCE_ID,
        //                                                                                        -1,
        //                                                                                        "Base Patience Decay");

        public const string PATIENCE_PER_PLAYER_ID = "playerPatienceMultiplier";
        //public const int PATIENCE_PER_PLAYER_INITIAL = 25;
        //public static readonly MenuPreference PatiencePerPlayerPreference = new MenuPreference(PATIENCE_PER_PLAYER_ID,
        //                                                                                       -2,
        //                                                                                       "Patience Decay Per Player");
        #endregion

        #region Order Preferences
        /*public const string ORDER_STARTER_MODIFIER_ID = "starterModifier";
        public const int ORDER_STARTER_MODIFIER_INITIAL = 25;
        public static readonly MenuPreference OrderStarterModifierPreference = new MenuPreference(ORDER_STARTER_MODIFIER_ID,
                                                                                                  -2,
                                                                                                  "Starter Chance");


        public const string ORDER_SIDES_MODIFIER_ID = "sidesModifier";
        public const int ORDER_SIDES_MODIFIER_INITIAL = 25;
        public static readonly MenuPreference OrderSidesModifierPreference = new MenuPreference(ORDER_SIDES_MODIFIER_ID,
                                                                                                -2,
                                                                                                "Sides Chance");


        public const string ORDER_DESSERT_MODIFIER_ID = "dessertModifier";
        public const int ORDER_DESSERT_MODIFIER_INITIAL = 25;
        public static readonly MenuPreference OrderDessertModifierPreference = new MenuPreference(ORDER_DESSERT_MODIFIER_ID,
                                                                                                  -2,
                                                                                                  "Dessert Chance");
        */
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
        //public static readonly int PLAYER_COLLISION_VIEW_ID = VariousUtils.GetID($"{Main.MOD_GUID}:{nameof(PlayerCollisionView)}");

        static int PlayersLayer = LayerMask.NameToLayer("Players");
        static int CustomersLayer = LayerMask.NameToLayer("Customers");
        static int DefaultLayer = LayerMask.NameToLayer("Default");
        static int WallsLayer = LayerMask.NameToLayer("Statics");
        static int[] Layers = new int[] { PlayersLayer, CustomersLayer, DefaultLayer, WallsLayer };
        public const string PLAYER_COLLISION_PREP_ID = "playerCollisionPrep";
        //public const int PLAYER_COLLISION_PREP_INITIAL = -1;
        //public static readonly MenuPreference PlayerCollisionPrepPreference = new MenuPreference(PLAYER_COLLISION_PREP_ID,
        //                                                                                     -1,
        //                                                                                     "Prep - Player Collides With");

        public const string PLAYER_COLLISION_ID = "playerCollisionDay";
        //public const int PLAYER_COLLISION_INITIAL = -1;
        //public static readonly MenuPreference PlayerCollisionPreference = new MenuPreference(PLAYER_COLLISION_ID,
        //                                                                                     -1,
        //                                                                                     "Day - Player Collides With");

        public const string PLAYER_OUT_OF_BOUNDS_ID = "playerEnforceBounds";
        //public const int PLAYER_OUT_OF_BOUNDS_INITIAL = 0;
        //public static readonly MenuPreference PlayerOutOfBoundsPreference = new MenuPreference(PLAYER_OUT_OF_BOUNDS_ID,
        //                                                                                       0,
        //                                                                                       "Allow Go Out Of Bounds");

        public const string PLAYER_SPEED_PREP_ID = "playerSpeedPrep";
        //public const int PLAYER_SPEED_PREP_INITIAL = 100;
        //public static readonly MenuPreference PlayerSpeedPrepPreference = new MenuPreference(PLAYER_SPEED_PREP_ID,
        //                                                                                     -1,
        //                                                                                     "Prep - Player Speed Modifier");

        public const string PLAYER_SPEED_ID = "playerSpeed";
        //public const int PLAYER_SPEED_INITIAL = 100;
        //public static readonly MenuPreference PlayerSpeedPreference = new MenuPreference(PLAYER_SPEED_ID,
        //                                                                                -1,
        //                                                                                "Day - Player Speed Modifier");
        #endregion

        #region Misc Preferences
        public const string FIRE_SPREAD_ID = "fireSpread";
        //public const int FIRE_SPREAD_INITIAL = 100;
        //public static readonly MenuPreference FireSpreadPreference = new MenuPreference(FIRE_SPREAD_ID,
        //                                                                                -2,
        //                                                                                "Fire Spread Modifier");

        /*
        public const string MESS_FACTOR_ID = "messFactor";
        public const int MESS_FACTOR_INITIAL = 100;
        public static readonly MenuPreference MessFactorPreference = new MenuPreference(MESS_FACTOR_ID,
                                                                                        -2,
                                                                                        "Mess Chance Multiplier");

        */
        public const string RESTART_ON_LOSS_ID = "offerRestartOnLoss";
        //public const int RESTART_ON_LOSS_INITIAL = 0;
        //public static readonly MenuPreference RestartOnLossPreference = new MenuPreference(RESTART_ON_LOSS_ID,
        //                                                                                   0,
        //                                                                                   "Restart Chance Upon Loss");

        public const string RESTART_FROM_PREP_END_ID = "restartFromPrepEnd";
        //public const int RESTART_FROM_PREP_END_INITIAL = 1;
        //public static readonly MenuPreference RestartFromPrepEndPreference = new MenuPreference(RESTART_FROM_PREP_END_ID,
        //                                                                                        1,
        //                                                                                        "Restart From End of Prep Phase");
        #endregion

        internal static PreferencesManager PrefManager;
        IntArrayGenerator IntArrayGen;
        IntArrayGenerator.IntToStringConversion ValueStringConversion;
        IntArrayGenerator.IntToStringConversion PercentStringConversion;

        public static Dictionary<string, int> DefaultValuesDict;

        //public static readonly Dictionary<string, int> AllCustomDifficultyValues = new Dictionary<string, int>();


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
            //foreach (MenuPreference menuPreference in AllCustomDifficultyPreferences.Values)
            //{
            //    menuPreference.Register(MOD_GUID);
            //    LogInfo($"Registered preference {MOD_GUID}:{menuPreference.PreferenceID}");
            //}
            //PreferenceUtils.Load();
            //SetupPreferences();
            //World.AddSystem(newStartNewDay);
            //World.GetExistingSystem(typeof(StartNewDay)).Enabled = false;
            //World.GetExistingSystem(typeof(NewStartNewDay)).Enabled = true;
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
                { PLAYER_PATIENCE_ENABLED_ID, 0 },
                { BASE_PLAYER_PATIENCE_ID, 75 },
                { PATIENCE_PER_PLAYER_ID, 25 },

                //{ ORDER_STARTER_MODIFIER_ID, OrderStarterModifierPreference },
                //{ ORDER_SIDES_MODIFIER_ID, OrderSidesModifierPreference },
                //{ ORDER_DESSERT_MODIFIER_ID, OrderDessertModifierPreference },

                //{ CHANGE_MIND_MODIFIER_ID, ChangeMindModifierPreference},
                //{ REPEAT_COURSE_MODIFIER_ID, RepeatCourseModifierPreference },
            
                { PLAYER_COLLISION_PREP_ID, -1 },
                { PLAYER_COLLISION_ID, -1 },
                { PLAYER_OUT_OF_BOUNDS_ID, 0 },
                { PLAYER_SPEED_ID , 100 },
                { PLAYER_SPEED_PREP_ID , 100 },


                { FIRE_SPREAD_ID, 100 },
                //{ MESS_FACTOR_ID, 100 },
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




            PrefManager.AddSubmenu("Shop", "shop");

            CreateIntOptionRow("Appliance Blueprint Count", SHOP_TOTAL_APPLIANCE_BLUEPRINTS_ID, 0, 15, 1, false, false);
            CreateIntOptionRow("Staple Appliance Blueprint Count", SHOP_STAPLE_BLUEPRINTS_ID, 0, 6, 1, true, false);

            PrefManager.AddSpacer();

            CreateIntOptionRow("Upgraded Chance", SHOP_UPGRADED_CHANCE_ID, 0, 1000, 25, true, true);

            PrefManager.AddSpacer();
            PrefManager.AddSpacer();

            PrefManager.SubmenuDone();




            PrefManager.AddSubmenu("Customers", "customers");

            CreateEnableDisableRow("Custom Group Count", PLAYER_CUSTOMERS_ENABLED_ID);
            CreateIntOptionRow("Base Customer Count", BASE_PLAYER_CUSTOMERS_ID, 0, 300, 10, false, true);
            CreateIntOptionRow("Customers Multiplier Per Player", CUSTOMERS_PER_PLAYER_ID, 0, 300, 10, false, true);

            PrefManager.AddSpacer();

            CreateEnableDisableRow("Custom Patience", PLAYER_PATIENCE_ENABLED_ID);
            CreateIntOptionRow("Base Patience Decay", BASE_PLAYER_PATIENCE_ID, 0, 300, 10, false, true);
            CreateIntOptionRow("Patience Decay Per Player", PATIENCE_PER_PLAYER_ID, 0, 300, 10, false, true);

            PrefManager.AddSpacer();
            PrefManager.AddSpacer();

            PrefManager.SubmenuDone();




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

            CreateEnableDisableRow("Allow Go Out Of Bounds", PLAYER_OUT_OF_BOUNDS_ID);

            PrefManager.AddSpacer();

            PrefManager.AddInfo("Note: The below settings may have undesired behaviour when used with other mods that affect player speed." +
                "Set all of them to \"Mod Compatibility\" which allows the other mod to overwrite Custom Difficulty. Use with Caution.");
            CreateIntOptionRow("Prep - Player Speed Modifier", PLAYER_SPEED_PREP_ID, 0, 500, 25, true, true, "Mod Compatibility", startOptionOverride: -1);
            CreateIntOptionRow("Day - Player Speed Modifier", PLAYER_SPEED_ID, 0, 500, 25, true, true, "Mod Compatibility", startOptionOverride: -1);

            PrefManager.AddSpacer();
            PrefManager.AddSpacer();

            PrefManager.SubmenuDone();




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

            CreateEnableDisableRow("Restart Chance Upon Loss", RESTART_ON_LOSS_ID, "On", "Off");
            CreateEnableDisableRow("Restart From End of Prep Phase", RESTART_FROM_PREP_END_ID);

            PrefManager.AddSpacer();
            PrefManager.AddSpacer();

            PrefManager.SubmenuDone();

            PrefManager.AddSpacer();
            PrefManager.AddSpacer();


            PrefManager.RegisterMenu(PreferencesManager.MenuType.PauseMenu);
        }

        protected override void OnUpdate()
        {
            //if (!PlayerCollisionViewPrefabAdded)
            //{
            //    PlayerCollisionViewPrefabAdded = AddPlayerCollisionViewComponentToPrefab();
            //}
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

        /*
        private bool AddPlayerCollisionViewComponentToPrefab()
        {
            if (TryGetSingletonEntity<SAssetDirectory>(out Entity assetDirEntity))
            {
                if (EntityManager.HasComponent<CViewDirectory>(assetDirEntity))
                {
                    AssetDirectory assetDir = EntityManager.GetSharedComponentData<CViewDirectory>(assetDirEntity).Directory;
                    if (!assetDir.ViewPrefabs.ContainsKey((ViewType)Main.PLAYER_COLLISION_VIEW_ID))
                    {
                        GameObject emptyGameObject = new GameObject();
                        GameObject viewObject = new GameObject();
                        viewObject.AddComponent<PlayerCollisionView>();
                        viewObject.transform.parent = emptyGameObject.transform;
                        assetDir.ViewPrefabs.Add((ViewType)Main.PLAYER_COLLISION_VIEW_ID, emptyGameObject);
                        emptyGameObject.SetActive(false);
                        return true;
                    }

                    //if (assetDir.ViewPrefabs.TryGetValue(ViewType.Player, out GameObject playerPrefab))
                    //{
                    //    GameObject playerCollisionManager = new GameObject("Player Collision Manager");
                    //    playerCollisionManager.AddComponent(typeof(PlayerCollisionView));
                    //    playerCollisionManager.transform.parent = playerPrefab.transform;
                    //    return true;
                    //}
                }
            }
            return false;
        }
        */
        //private void SetupPreferences()
        //{
        //    Events.PreferenceMenu_MainMenu_CreateSubmenusEvent += (s, args) =>
        //    {
        //        args.Menus.Add(typeof(CustomDifficultyPreferences<MainMenuAction>), new CustomDifficultyPreferences<MainMenuAction>(args.Container, args.Module_list));
        //        args.Menus.Add(typeof(ShopSubmenu<MainMenuAction>), new ShopSubmenu<MainMenuAction>(args.Container, args.Module_list));
        //        args.Menus.Add(typeof(CustomersSubmenu<MainMenuAction>), new CustomersSubmenu<MainMenuAction>(args.Container, args.Module_list));
        //        //args.Menus.Add(typeof(OrderSubmenu<MainMenuAction>), new OrderSubmenu<MainMenuAction>(args.Container, args.Module_list));
        //        //args.Menus.Add(typeof(OrderCardsSubmenu<MainMenuAction>), new OrderCardsSubmenu<MainMenuAction>(args.Container, args.Module_list));
        //        args.Menus.Add(typeof(PlayerSubmenu<MainMenuAction>), new PlayerSubmenu<MainMenuAction>(args.Container, args.Module_list));
        //        args.Menus.Add(typeof(MiscSubmenu<MainMenuAction>), new MiscSubmenu<MainMenuAction>(args.Container, args.Module_list));
        //    };
        //    ModsPreferencesMenu<MainMenuAction>.RegisterMenu(MOD_NAME, typeof(CustomDifficultyPreferences<MainMenuAction>), typeof(MainMenuAction));

        //    //Setting Up For Pause Menu
        //    Events.PreferenceMenu_PauseMenu_CreateSubmenusEvent += (s, args) =>
        //    {
        //        args.Menus.Add(typeof(CustomDifficultyPreferences<PauseMenuAction>), new CustomDifficultyPreferences<PauseMenuAction>(args.Container, args.Module_list));
        //        args.Menus.Add(typeof(ShopSubmenu<PauseMenuAction>), new ShopSubmenu<PauseMenuAction>(args.Container, args.Module_list));
        //        args.Menus.Add(typeof(CustomersSubmenu<PauseMenuAction>), new CustomersSubmenu<PauseMenuAction>(args.Container, args.Module_list));
        //        //args.Menus.Add(typeof(OrderSubmenu<PauseMenuAction>), new OrderSubmenu<PauseMenuAction>(args.Container, args.Module_list));
        //        //args.Menus.Add(typeof(OrderCardsSubmenu<PauseMenuAction>), new OrderCardsSubmenu<PauseMenuAction>(args.Container, args.Module_list));
        //        args.Menus.Add(typeof(PlayerSubmenu<PauseMenuAction>), new PlayerSubmenu<PauseMenuAction>(args.Container, args.Module_list));
        //        args.Menus.Add(typeof(MiscSubmenu<PauseMenuAction>), new MiscSubmenu<PauseMenuAction>(args.Container, args.Module_list));
        //    };
        //    ModsPreferencesMenu<PauseMenuAction>.RegisterMenu(MOD_NAME, typeof(CustomDifficultyPreferences<PauseMenuAction>), typeof(PauseMenuAction));
        //}

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
