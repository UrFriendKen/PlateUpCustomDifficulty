using Kitchen;
using System;
using System.Collections.Generic;
using System.Reflection;
using Unity.Entities;

namespace KitchenCustomDifficulty
{
    [UpdateAfter(typeof(StartNewDay))]
    [UpdateInGroup(typeof(TimeManagementGroup))]
    internal class GroupCountController : NightSystem
    {
        private static Type sCacheHashType = typeof(CreateCustomerSchedule).GetNestedType("SCacheHash", BindingFlags.NonPublic);
        EntityQuery sCacheHashQuery;

        protected override void Initialise()
        {
            base.Initialise();
            sCacheHashQuery = GetEntityQuery(sCacheHashType);
        }

        protected override void OnUpdate()
        {
            if (Main.PrefManager.Get<int>(Main.PLAYER_CUSTOMERS_ENABLED_ID) == 1)
            {
                EntityManager.DestroyEntity(sCacheHashQuery);
            }
        }
    }
}
