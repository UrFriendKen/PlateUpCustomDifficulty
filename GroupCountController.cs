using Kitchen;
using KitchenCustomDifficulty.Patches;
using System;
using System.Reflection;
using Unity.Entities;

namespace KitchenCustomDifficulty
{
    [UpdateAfter(typeof(StartNewDay))]
    [UpdateInGroup(typeof(TimeManagementGroup))]
    internal class GroupCountController : RestaurantSystem
    {
        private struct CustomerSettings
        {
            public int Enabled;
            public int BaseMultiplier;
            public int PerPlayerMultiplier;
            public int MinGroupSize;
            public int MaxGroupSize;

            public bool HasChanged(CustomerSettings other)
            {
                return
                    Enabled != other.Enabled ||
                    BaseMultiplier != other.BaseMultiplier ||
                    PerPlayerMultiplier != other.BaseMultiplier ||
                    MinGroupSize != other.MinGroupSize ||
                    MaxGroupSize != other.MaxGroupSize;
            }
        }

        private CustomerSettings prevSettings;

        private static Type sCacheHashType = typeof(CreateCustomerSchedule).GetNestedType("SCacheHash", BindingFlags.NonPublic);
        EntityQuery sCacheHashQuery;

        protected override void Initialise()
        {
            base.Initialise();
            sCacheHashQuery = GetEntityQuery(sCacheHashType);
            prevSettings = default;
        }

        protected override void OnUpdate()
        {
            if (!Has<SIsNightTime>())
            {
                prevSettings = default;
                return;
            }

            CustomerSettings customerSettings = new CustomerSettings
            {
                Enabled = Main.PrefManager.Get<int>(Main.PLAYER_CUSTOMERS_ENABLED_ID),
                BaseMultiplier = Main.PrefManager.Get<int>(Main.BASE_PLAYER_CUSTOMERS_ID),
                PerPlayerMultiplier = Main.PrefManager.Get<int>(Main.CUSTOMERS_PER_PLAYER_ID),
                MinGroupSize = Main.PrefManager.Get<int>(Main.CUSTOMERS_MIN_GROUP_SIZE_ID),
                MaxGroupSize = Main.PrefManager.Get<int>(Main.CUSTOMERS_MAX_GROUP_SIZE_ID)
            };

            if (prevSettings.HasChanged(customerSettings))
            {
                EntityManager.DestroyEntity(sCacheHashQuery);
                prevSettings = customerSettings;
            }
        }
    }
}
