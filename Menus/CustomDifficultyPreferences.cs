using Kitchen.Modules;
using Kitchen;
using KitchenLib.Utils;
using KitchenLib;
using System.Collections.Generic;
using UnityEngine;

namespace KitchenCustomDifficulty.Menus
{
	public static class PreferencesHelper
	{
        public static void Preference_OnChanged(string preferenceID, int f)
        {
            PreferenceUtils.Get<KitchenLib.IntPreference>(Main.MOD_GUID, preferenceID).Value = f;
            PreferenceUtils.Save();
        }
    }

	public class CustomDifficultyPreferences<T> : KLMenu<T>
	{
		public CustomDifficultyPreferences(Transform container, ModuleList module_list) : base(container, module_list)
		{
		}

		public override void Setup(int player_id)
		{
			AddLabel("Custom Difficulty");

			New<SpacerElement>();

			AddSubmenuButton("Shop", typeof(ShopSubmenu<T>));

			AddSubmenuButton("Customers", typeof(CustomersSubmenu<T>));

            //AddSubmenuButton("Orders", typeof(OrderSubmenu<T>));

            // AddSubmenuButton("Order Cards", typeof(OrderCardsSubmenu<T>));

            AddSubmenuButton("Player", typeof(PlayerSubmenu<T>));

            AddSubmenuButton("Miscellaneous", typeof(MiscSubmenu<T>));

			New<SpacerElement>();
			New<SpacerElement>();

			AddButton(base.Localisation["MENU_BACK_SETTINGS"], delegate
			{
				RequestPreviousMenu();
			});
		}
	}

	public class ShopSubmenu<T> : KLMenu<T>
	{
		public ShopSubmenu(Transform container, ModuleList module_list) : base(container, module_list)
		{
		}

		private Option<int> ShopTotalApplianceBlueprints;
		private Option<int> ShopStapleBlueprints;
		private Option<int> ShopUpgradedChance;
		// private Option<int> ShopIncreaseRerollCost;

		public override void Setup(int player_id)
		{
			SetupBlueprintPreferences();

			New<SpacerElement>();

			SetupMiscShopPreferences();

			New<SpacerElement>();
			New<SpacerElement>();

			AddButton(base.Localisation["MENU_BACK_SETTINGS"], delegate
			{
				RequestPreviousMenu();
			});
		}

		private void SetupBlueprintPreferences()
		{
			List<int> shopTotalApplianceBlueprints_ValList = new List<int>() { -1 };
			List<string> shopTotalApplianceBlueprints_StrList = new List<string>() { $"Default ({Main.SHOP_TOTAL_APPLIANCE_BLUEPRINTS_INITIAL})" };

			List<int> shopStapleBlueprints_ValList = new List<int>() { -2, -1 };
			List<string> shopStapleBlueprints_StrList = new List<string>() { "Vanilla", $"Default ({Main.SHOP_STAPLE_BLUEPRINTS_INITIAL})" };

			for (int i = 0; i < 16; i++)
			{
				shopTotalApplianceBlueprints_ValList.Add(i);
				shopTotalApplianceBlueprints_StrList.Add($"{i}");

				if (i <= 6)
				{
					shopStapleBlueprints_ValList.Add(i);
					shopStapleBlueprints_StrList.Add($"{i}");
				}
			}

			this.ShopTotalApplianceBlueprints = new Option<int>(
				shopTotalApplianceBlueprints_ValList,
				Main.ShopTotalApplianceBlueprintsPreference.Load(Main.MOD_GUID),
				shopTotalApplianceBlueprints_StrList);

			AddLabel(Main.ShopTotalApplianceBlueprintsPreference.PreferenceDisplayText);
			Add<int>(this.ShopTotalApplianceBlueprints).OnChanged += delegate (object _, int f)
			{
                PreferencesHelper.Preference_OnChanged(Main.SHOP_TOTAL_APPLIANCE_BLUEPRINTS_ID, f);
			};

			this.ShopStapleBlueprints = new Option<int>(
				shopStapleBlueprints_ValList,
				Main.ShopStapleBlueprintsPreference.Load(Main.MOD_GUID),
				shopStapleBlueprints_StrList);

			AddLabel(Main.ShopStapleBlueprintsPreference.PreferenceDisplayText);
			Add<int>(this.ShopStapleBlueprints).OnChanged += delegate (object _, int f)
            {
                PreferencesHelper.Preference_OnChanged(Main.SHOP_STAPLE_BLUEPRINTS_ID, f);
			};
		}
		
		private void SetupMiscShopPreferences()
		{
			int shopUpgradedChanceStepPercentage = 25;
			List<int> shopUpgradedChance_ValList = new List<int>() { -2, -1 };
			List<string> shopUpgradedChance_StrList = new List<string>() { "Vanilla", $"Default ({Main.SHOP_UPGRADED_CHANCE_INITIAL}%)" };
			for (int i = 0; i < (1000 / shopUpgradedChanceStepPercentage) + 1; i++)
			{
				int val = i * shopUpgradedChanceStepPercentage;
				shopUpgradedChance_ValList.Add(val);
				shopUpgradedChance_StrList.Add($"{val}%");
			}

			this.ShopUpgradedChance = new Option<int>(
				shopUpgradedChance_ValList,
				Main.ShopUpgradedChancePreference.Load(Main.MOD_GUID),
				shopUpgradedChance_StrList);

			AddLabel(Main.ShopUpgradedChancePreference.PreferenceDisplayText);
			Add<int>(this.ShopUpgradedChance).OnChanged += delegate (object _, int f)
			{
                PreferencesHelper.Preference_OnChanged(Main.SHOP_UPGRADED_CHANCE_ID, f);
			};

            //int shopIncreaseRerollCostStep = 10;
            //List<int> ShopIncreaseRerollCost_ValList = new List<int>() { -1 };
            //List<string> ShopIncreaseRerollCost_StrList = new List<string>() { $"Default ({Main.SHOP_INCREASE_REROLL_COST_INITIAL})" };
            //for (int i = 0; i < (500 / shopIncreaseRerollCostStep) + 1; i++)
            //{
            //	int val = i * shopIncreaseRerollCostStep;
            //	ShopIncreaseRerollCost_ValList.Add(val);
            //	ShopIncreaseRerollCost_StrList.Add($"{val}");
            //}

            //this.ShopIncreaseRerollCost = new Option<int>(
            //	ShopIncreaseRerollCost_ValList,
            //	Main.ShopIncreaseRerollCostPreference.Load(Main.MOD_GUID),
            //	ShopIncreaseRerollCost_StrList);

            //AddLabel(Main.ShopIncreaseRerollCostPreference.PreferenceDisplayText);
            //Add<int>(this.ShopIncreaseRerollCost).OnChanged += delegate (object _, int f)
            //{
            //	PreferencesHelper.Preference_OnChanged(Main.SHOP_INCREASE_REROLL_COST_ID, f);
            //};

        }

    }

	public class CustomersSubmenu<T> : KLMenu<T>
    {
        public CustomersSubmenu(Transform container, ModuleList module_list) : base(container, module_list)
        {
        }

		private Option<int> PlayerCustomersEnabled;
		private Option<int> CustomersPerPlayer;
		private Option<int> BasePlayerCustomers;

		private Option<int> PlayerPatienceEnabled;
		private Option<int> PatiencePerPlayer;
		private Option<int> BasePlayerPatience;

		public override void Setup(int player_id)
		{
			SetupCustomersPreferences();

			New<SpacerElement>();

			SetupPatiencePreferences();

			New<SpacerElement>();
			New<SpacerElement>();

			AddButton(base.Localisation["MENU_BACK_SETTINGS"], delegate
			{
				RequestPreviousMenu();
			});
		}

		private void SetupCustomersPreferences()
		{
			List<int> playerCustomersEnabled_ValList = new List<int>() { 0, 1 };
			List<string> playerCustomersEnabled_StrList = new List<string>() { "Disabled", "Enabled" };
			this.PlayerCustomersEnabled = new Option<int>(
				playerCustomersEnabled_ValList,
				Main.PlayerCustomersEnabledPreference.Load(Main.MOD_GUID),
				playerCustomersEnabled_StrList);

			AddLabel(Main.PlayerCustomersEnabledPreference.PreferenceDisplayText);
			Add<int>(this.PlayerCustomersEnabled).OnChanged += delegate (object _, int f)
			{
                PreferencesHelper.Preference_OnChanged(Main.PLAYER_CUSTOMERS_ENABLED_ID, f);
			};


			int basePlayerCustomersStepPercentage = 10;
			List<int> basePlayerCustomersPercentage_ValList = new List<int>() { -1 };
			List<string> basePlayerCustomersPercentage_StrList = new List<string>() { $"Default ({Main.BASE_PLAYER_CUSTOMERS_INITIAL}%)" };
			for (int i = 0; i < (300 / basePlayerCustomersStepPercentage) + 1; i++)
			{
				int val = i * basePlayerCustomersStepPercentage;
				basePlayerCustomersPercentage_ValList.Add(val);
				basePlayerCustomersPercentage_StrList.Add($"{val}%");
			}

			this.BasePlayerCustomers = new Option<int>(
				basePlayerCustomersPercentage_ValList,
				Main.BasePlayerCustomersPreference.Load(Main.MOD_GUID),
				basePlayerCustomersPercentage_StrList);

			AddLabel(Main.BasePlayerCustomersPreference.PreferenceDisplayText);
			Add<int>(this.BasePlayerCustomers).OnChanged += delegate (object _, int f)
            {
                PreferencesHelper.Preference_OnChanged(Main.BASE_PLAYER_CUSTOMERS_ID, f);
            };




			int customersPerPlayerStepPercentage = 10;
			List<int> customersPerPlayerStepPercentage_ValList = new List<int>() { -1 };
			List<string> customersPerPlayerStepPercentage_StrList = new List<string>() { $"Default ({Main.CUSTOMERS_PER_PLAYER_INITIAL}%)" };

			for (int i = 0; i < (300 / customersPerPlayerStepPercentage) + 1; i++)
			{
				int val = i * customersPerPlayerStepPercentage;
				customersPerPlayerStepPercentage_ValList.Add(val);
				customersPerPlayerStepPercentage_StrList.Add($"{val}%");
			}

			this.CustomersPerPlayer = new Option<int>(
				customersPerPlayerStepPercentage_ValList,
				Main.CustomersPerPlayerPreference.Load(Main.MOD_GUID),
				customersPerPlayerStepPercentage_StrList);

			AddLabel(Main.CustomersPerPlayerPreference.PreferenceDisplayText);
			Add<int>(this.CustomersPerPlayer).OnChanged += delegate (object _, int f)
            {
                PreferencesHelper.Preference_OnChanged(Main.CUSTOMERS_PER_PLAYER_ID, f);
            };
		}

		private void SetupPatiencePreferences()
		{
			List<int> playerPatienceEnabled_ValList = new List<int>() { 0, 1 };
			List<string> playerPatienceEnabled_StrList = new List<string>() { "Disabled", "Enabled" };
			this.PlayerPatienceEnabled = new Option<int>(
				playerPatienceEnabled_ValList,
				Main.PlayerPatienceEnabledPreference.Load(Main.MOD_GUID),
				playerPatienceEnabled_StrList);

			AddLabel(Main.PlayerPatienceEnabledPreference.PreferenceDisplayText);
			Add<int>(this.PlayerPatienceEnabled).OnChanged += delegate (object _, int f)
            {
                PreferencesHelper.Preference_OnChanged(Main.PLAYER_PATIENCE_ENABLED_ID, f);
            };



			int basePlayerPatienceStepPercentage = 10;
			List<int> basePlayerPatiencePercentage_ValList = new List<int>() { -1 };
			List<string> basePlayerPatiencePercentage_StrList = new List<string>() { $"Default ({Main.BASE_PLAYER_PATIENCE_INITIAL}%)" };
			for (int i = 0; i < (300 / basePlayerPatienceStepPercentage) + 1; i++)
			{
				int val = i * basePlayerPatienceStepPercentage;
				basePlayerPatiencePercentage_ValList.Add(val);
				basePlayerPatiencePercentage_StrList.Add($"{val}%");
			}

			this.BasePlayerPatience = new Option<int>(
				basePlayerPatiencePercentage_ValList,
				Main.BasePlayerPatiencePreference.Load(Main.MOD_GUID),
				basePlayerPatiencePercentage_StrList);

			AddLabel(Main.BasePlayerPatiencePreference.PreferenceDisplayText);
			Add<int>(this.BasePlayerPatience).OnChanged += delegate (object _, int f)
            {
                PreferencesHelper.Preference_OnChanged(Main.BASE_PLAYER_PATIENCE_ID, f);
            };




			int patiencePerPlayerStepPercentage = 10;
			List<int> patiencePerPlayerStepPercentage_ValList = new List<int>() { -1 };
			List<string> patiencePerPlayerStepPercentage_StrList = new List<string>() { $"Default ({Main.PATIENCE_PER_PLAYER_INITIAL}%)" };

			for (int i = 0; i < (300 / patiencePerPlayerStepPercentage) + 1; i++)
			{
				int val = i * patiencePerPlayerStepPercentage;
				patiencePerPlayerStepPercentage_ValList.Add(val);
				patiencePerPlayerStepPercentage_StrList.Add($"{val}%");
			}

			this.PatiencePerPlayer = new Option<int>(
				patiencePerPlayerStepPercentage_ValList,
				Main.PatiencePerPlayerPreference.Load(Main.MOD_GUID),
				patiencePerPlayerStepPercentage_StrList);

			AddLabel(Main.PatiencePerPlayerPreference.PreferenceDisplayText);
			Add<int>(this.PatiencePerPlayer).OnChanged += delegate (object _, int f)
            {
                PreferencesHelper.Preference_OnChanged(Main.PATIENCE_PER_PLAYER_ID, f);
            };
		}
	}

	public class OrderSubmenu<T> : KLMenu<T>
	{
		public OrderSubmenu(Transform container, ModuleList module_list) : base(container, module_list)
		{
		}

        //private Option<int> OrderStarterModifier;
        //private Option<int> OrderSidesModifier;
        //private Option<int> OrderDessertModifier;

        //public override void Setup(int player_id)
        //{
        //	SetupOrderPreferences();

        //	New<SpacerElement>();
        //	New<SpacerElement>();

        //	AddButton(base.Localisation["MENU_BACK_SETTINGS"], delegate
        //	{
        //		RequestPreviousMenu();
        //	});
        //}

        //private void SetupOrderPreferences()
        //{
        //	int orderChanceStepPercent = 10;

        //	List<int> orderStarterModifier_ValList = new List<int>() { -2, -1 };
        //	List<string> orderStarterModifier_StrList = new List<string>() { "Vanilla", $"Default ({Main.ORDER_STARTER_MODIFIER_INITIAL}%)" };

        //	List<int> orderSidesModifier_ValList = new List<int>() { -2, -1 };
        //	List<string> orderSidesModifier_StrList = new List<string>() { "Vanilla", $"Default ({Main.ORDER_SIDES_MODIFIER_INITIAL}%)" };

        //	List<int> orderDessertModifier_ValList = new List<int>() { -2, -1 };
        //	List<string> orderDessertModifier_StrList = new List<string>() { "Vanilla", $"Default ({Main.ORDER_DESSERT_MODIFIER_INITIAL}%)" };

        //	for (int i = 0; i < (100 / orderChanceStepPercent) + 1; i++)
        //	{
        //		int val = i * orderChanceStepPercent;
        //		string displayString = $"{val}%";

        //		orderStarterModifier_ValList.Add(val);
        //		orderStarterModifier_StrList.Add(displayString);

        //		orderSidesModifier_ValList.Add(val);
        //		orderSidesModifier_StrList.Add(displayString);

        //		orderDessertModifier_ValList.Add(val);
        //		orderDessertModifier_StrList.Add(displayString);
        //	}

        //	this.OrderStarterModifier = new Option<int>(
        //		orderStarterModifier_ValList,
        //		Main.OrderStarterModifierPreference.Load(Main.MOD_GUID),
        //		orderStarterModifier_StrList);

        //	AddLabel(Main.OrderStarterModifierPreference.PreferenceDisplayText);
        //	Add<int>(this.OrderStarterModifier).OnChanged += delegate (object _, int f)
        //	{
        //		PreferencesHelper.Preference_OnChanged(Main.ORDER_STARTER_MODIFIER_ID, f);
        //	};



        //	this.OrderSidesModifier = new Option<int>(
        //		orderSidesModifier_ValList,
        //		Main.OrderSidesModifierPreference.Load(Main.MOD_GUID),
        //		orderSidesModifier_StrList);

        //	AddLabel(Main.OrderSidesModifierPreference.PreferenceDisplayText);
        //	Add<int>(this.OrderSidesModifier).OnChanged += delegate (object _, int f)
        //	{
        //		PreferencesHelper.Preference_OnChanged(Main.ORDER_SIDES_MODIFIER_ID, f);
        //	};



        //	this.OrderDessertModifier = new Option<int>(
        //		orderDessertModifier_ValList,
        //		Main.OrderDessertModifierPreference.Load(Main.MOD_GUID),
        //		orderDessertModifier_StrList);

        //	AddLabel(Main.OrderDessertModifierPreference.PreferenceDisplayText);
        //	Add<int>(this.OrderDessertModifier).OnChanged += delegate (object _, int f)
        //	{
        //		PreferencesHelper.Preference_OnChanged(Main.ORDER_DESSERT_MODIFIER_ID, f);
        //	};
        //}
    }

    public class OrderCardsSubmenu<T> : KLMenu<T>
	{
		public OrderCardsSubmenu(Transform container, ModuleList module_list) : base(container, module_list)
		{
		}
        /*
		private Option<int> ChangeMindModifier;
		private Option<int> RepeatCourseModifier;

		public override void Setup(int player_id)
		{
			SetupOrderCardsPreferences();

			New<SpacerElement>();
			New<SpacerElement>();

			AddButton(base.Localisation["MENU_BACK_SETTINGS"], delegate
			{
				RequestPreviousMenu();
			});
		}
		
		private void SetupOrderCardsPreferences()
		{
			int orderCardsStepPercent = 5;

			List<int> changeMindModifier_ValList = new List<int>() { -2, -1 };
			List<string> changeMindModifier_StrList = new List<string>() { "Vanilla", $"Default ({Main.CHANGE_MIND_MODIFIER_INITIAL}%)" };

			List<int> repeatCourseModifier_ValList = new List<int>() { -2, -1 };
			List<string> repeatCourseModifier_StrList = new List<string>() { "Vanilla", $"Default ({Main.REPEAT_COURSE_MODIFIER_INITIAL}%)" };

			for (int i = 0; i < (100 / orderCardsStepPercent) + 1; i++)
			{
				int val = i * orderCardsStepPercent;
				string displayString = $"{val}%";

				if (val <= 75)
				{
					changeMindModifier_ValList.Add(val);
					changeMindModifier_StrList.Add(displayString);
				}

				repeatCourseModifier_ValList.Add(val);
				repeatCourseModifier_StrList.Add(displayString);
			}

			
			this.ChangeMindModifier = new Option<int>(
				changeMindModifier_ValList,
				Main.ChangeMindModifierPreference.Load(Main.MOD_GUID),
				changeMindModifier_StrList);

			AddLabel(Main.ChangeMindModifierPreference.PreferenceDisplayText);
			Add<int>(this.ChangeMindModifier).OnChanged += delegate (object _, int f)
			{
				PreferencesHelper.Preference_OnChanged(Main.CHANGE_MIND_MODIFIER_ID, f);
			};


			
			this.RepeatCourseModifier = new Option<int>(
				repeatCourseModifier_ValList,
				Main.RepeatCourseModifierPreference.Load(Main.MOD_GUID),
				repeatCourseModifier_StrList);

			AddLabel(Main.RepeatCourseModifierPreference.PreferenceDisplayText);
			Add<int>(this.RepeatCourseModifier).OnChanged += delegate (object _, int f)
			{
				PreferencesHelper.Preference_OnChanged(Main.REPEAT_COURSE_MODIFIER_ID, f);
			};
		}
	*/
    }

    public class PlayerSubmenu<T> : KLMenu<T>
    {
        public PlayerSubmenu(Transform container, ModuleList module_list) : base(container, module_list)
        {
        }

		private Option<int> PlayerCollisionPrep;
		private Option<int> PlayerCollision;
        private Option<int> PlayerOutOfBounds;
        private Option<int> PlayerSpeedPrep;
        private Option<int> PlayerSpeed;

        public override void Setup(int player_id)
        {
            SetupPlayerPreferences();

            New<SpacerElement>();
            New<SpacerElement>();

            AddButton(base.Localisation["MENU_BACK_SETTINGS"], delegate
            {
                RequestPreviousMenu();
            });
        }

        private void SetupPlayerPreferences()
        {
            List<int> playerCollision_ValList = new List<int>() { -1, 0, 1, 2, 3 };
            List<string> playerCollision_StrList = new List<string>() { "Everything", "Everything except players", "Appliances and Walls Only", "Walls Only", "Nothing" };

            this.PlayerCollisionPrep = new Option<int>(
                playerCollision_ValList,
                Main.PlayerCollisionPrepPreference.Load(Main.MOD_GUID),
                playerCollision_StrList);

            AddLabel(Main.PlayerCollisionPrepPreference.PreferenceDisplayText);
            Add<int>(this.PlayerCollisionPrep).OnChanged += delegate (object _, int f)
            {
                PreferencesHelper.Preference_OnChanged(Main.PLAYER_COLLISION_PREP_ID, f);
            };

            this.PlayerCollision = new Option<int>(
                playerCollision_ValList,
                Main.PlayerCollisionPreference.Load(Main.MOD_GUID),
                playerCollision_StrList);

            AddLabel(Main.PlayerCollisionPreference.PreferenceDisplayText);
            Add<int>(this.PlayerCollision).OnChanged += delegate (object _, int f)
            {
                PreferencesHelper.Preference_OnChanged(Main.PLAYER_COLLISION_ID, f);
            };

            List<int> playerOutOfBounds_ValList = new List<int>() { 0, 1 };
            List<string> playerOutOfBounds_StrList = new List<string>() { "Disabled", "Enabled" };

            this.PlayerOutOfBounds = new Option<int>(
                playerOutOfBounds_ValList,
                Main.PlayerOutOfBoundsPreference.Load(Main.MOD_GUID),
                playerOutOfBounds_StrList);

            AddLabel(Main.PlayerOutOfBoundsPreference.PreferenceDisplayText);
            Add<int>(this.PlayerOutOfBounds).OnChanged += delegate (object _, int f)
            {
                PreferencesHelper.Preference_OnChanged(Main.PLAYER_OUT_OF_BOUNDS_ID, f);
            };

            New<SpacerElement>();

            List<int> playerSpeed_ValList = new List<int>() { -2 , -1 };
            List<string> playerSpeed_StrList = new List<string>() { "Mod Compatibility", $"Default ({Main.PLAYER_SPEED_INITIAL}%)" };
            int playerSpeedStepPercentage = 25;
            for (int i = 0; i < (500 / playerSpeedStepPercentage) + 1; i++)
            {
                int val = i * playerSpeedStepPercentage;
                playerSpeed_ValList.Add(val);
                playerSpeed_StrList.Add($"{val}%");
            }

            this.PlayerSpeedPrep = new Option<int>(
                playerSpeed_ValList,
                Main.PlayerSpeedPrepPreference.Load(Main.MOD_GUID),
                playerSpeed_StrList);

            AddInfo("Note: The below settings may have undesired behaviour when used with other mods that affect player speed.\nSet all of them to \"Mod Compatibility\" which allows the other mod to overwrite Custom Difficulty. Use with Caution.");
            AddLabel(Main.PlayerSpeedPrepPreference.PreferenceDisplayText); 
			Add<int>(this.PlayerSpeedPrep).OnChanged += delegate (object _, int f)
            {
                PreferencesHelper.Preference_OnChanged(Main.PLAYER_SPEED_PREP_ID, f);
            };

            this.PlayerSpeed = new Option<int>(
                playerSpeed_ValList,
                Main.PlayerSpeedPreference.Load(Main.MOD_GUID),
                playerSpeed_StrList);

            AddLabel(Main.PlayerSpeedPreference.PreferenceDisplayText);
            Add<int>(this.PlayerSpeed).OnChanged += delegate (object _, int f)
			{
                PreferencesHelper.Preference_OnChanged(Main.PLAYER_SPEED_ID, f);
			};
        }
    }

    public class MiscSubmenu<T> : KLMenu<T>
	{
		public MiscSubmenu(Transform container, ModuleList module_list) : base(container, module_list)
		{
		}

		private Option<int> FireSpread;
		//private Option<int> MessFactor;
		private Option<int> RestartOnLoss;
		private Option<int> RestartFromPrepEnd;

		public override void Setup(int player_id)
		{
			SetupMiscPreferences();

			New<SpacerElement>();
			New<SpacerElement>();

			AddButton(base.Localisation["MENU_BACK_SETTINGS"], delegate
			{
				RequestPreviousMenu();
			});
		}

		private void SetupMiscPreferences()
		{
			List<int> fireSpread_ValList = new List<int>() { -1 };
			List<string> fireSpread_StrList = new List<string>() { $"Default ({Main.FIRE_SPREAD_INITIAL}%)" };
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

			this.FireSpread = new Option<int>(
				fireSpread_ValList,
				Main.FireSpreadPreference.Load(Main.MOD_GUID),
				fireSpread_StrList);

			AddLabel(Main.FireSpreadPreference.PreferenceDisplayText);
			Add<int>(this.FireSpread).OnChanged += delegate (object _, int f)
            {
                PreferencesHelper.Preference_OnChanged(Main.FIRE_SPREAD_ID, f);
			};



			//int messFactorStepPercentage = 25;
			//List<int> messFactor_ValList = new List<int>() { -2, -1 };
			//List<string> messFactor_StrList = new List<string>() { "Vanilla", $"Default ({Main.MESS_FACTOR_INITIAL}%)" };
			//for (int i = 0; i < (300 / messFactorStepPercentage) + 1; i++)
			//{
			//	int val = i * messFactorStepPercentage;
			//	messFactor_ValList.Add(val);
			//	messFactor_StrList.Add($"{val}%");
			//}

			//this.MessFactor = new Option<int>(
			//	messFactor_ValList,
			//	Main.MessFactorPreference.Load(Main.MOD_GUID),
			//	messFactor_StrList);

			//AddLabel(Main.MessFactorPreference.PreferenceDisplayText);
			//Add<int>(this.MessFactor).OnChanged += delegate (object _, int f)
			//{
			//	PreferenceUtils.Get<KitchenLib.IntPreference>(Main.MOD_GUID, Main.MESS_FACTOR_ID).Value = f;
			//};



			List<int> restartOnLoss_ValList = new List<int>() { 0, 1 };
			List<string> restartOnLoss_StrList = new List<string>() { "Off", "On" };

			this.RestartOnLoss = new Option<int>(
				restartOnLoss_ValList,
				Main.RestartOnLossPreference.Load(Main.MOD_GUID),
				restartOnLoss_StrList);

			AddLabel(Main.RestartOnLossPreference.PreferenceDisplayText);
			Add<int>(this.RestartOnLoss).OnChanged += delegate (object _, int f)
			{
                PreferencesHelper.Preference_OnChanged(Main.RESTART_ON_LOSS_ID, f);
			};


			List<int> restartFromPrepEnd_ValList = new List<int>() { 0, 1 };
			List<string> restartFromPrepEnd_StrList = new List<string>() { "Disabled", "Enabled" };

			this.RestartFromPrepEnd = new Option<int>(
				restartFromPrepEnd_ValList,
				Main.RestartFromPrepEndPreference.Load(Main.MOD_GUID),
				restartFromPrepEnd_StrList);

			AddLabel(Main.RestartFromPrepEndPreference.PreferenceDisplayText);
			AddInfo("Enabling saves the changes you made during the previous prep phase.");
			Add<int>(this.RestartFromPrepEnd).OnChanged += delegate (object _, int f)
			{
				PreferencesHelper.Preference_OnChanged(Main.RESTART_FROM_PREP_END_ID, f);
			};
		}
    }
}