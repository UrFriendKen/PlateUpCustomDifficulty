﻿using Kitchen;
using KitchenMods;
using Unity.Entities;
using UnityEngine;

namespace KitchenCustomDifficulty
{
    public class PatchController : GameSystemBase, IModSystem
    {
        private static PatchController _instance;
        protected override void Initialise()
        {
            base.Initialise();
            _instance = this;
        }

        protected override void OnUpdate()
        {
        }

        internal static void UpdateCustomerSpeed(Entity customerEntity)
        {
            if (_instance == null)
                return;

            CCustomer customer = _instance.EntityManager.GetComponentData<CCustomer>(customerEntity);

            int multiplier = Main.PrefSysManager.Get<int>(Main.CUSTOMER_SPEED_ID);
            if (multiplier == -1)
            {
                customer.Speed *= Main.DefaultValuesDict[Main.CUSTOMER_SPEED_ID] / 100f;
            }
            else
            {
                customer.Speed *= multiplier / 100f;
            }
            _instance.EntityManager.SetComponentData(customerEntity, customer);
        }

        internal static bool CustomOfferRestart(out bool shouldRunOriginal)
        {
            if (_instance == null || !_instance.TryGetSingleton(out SKitchenStatus kitchenStatus))
            {
                shouldRunOriginal = false;
                return true;
            }

            switch (Main.PrefSysManager.Get<int>(Main.RESTART_ON_LOSS_ID))
            {
                case -1:
                    shouldRunOriginal = false;
                    return false;
                case 1:
                    shouldRunOriginal = false;
                    _instance.World.Add(new COfferRestartDay
                    {
                        Reason = LossReason.Patience
                    });
                    _instance.Set(new SKitchenStatus
                    {
                        RemainingLives = kitchenStatus.TotalLives,
                        TotalLives = kitchenStatus.TotalLives
                    });
                    return true;
                default:
                    shouldRunOriginal = true;
                    return false;
            }
        }

        internal static bool TryGetBounds(out Bounds bounds)
        {
            bounds = default;
            if (_instance == null || !_instance.TryGetSingletonEntity<SLayout>(out Entity singletonEntity) || !_instance.Require(singletonEntity, out CBounds cBounds))
                return false;

            bounds = cBounds.Bounds;
            return true;
        }
    }
}
