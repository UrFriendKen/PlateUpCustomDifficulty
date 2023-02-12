using Kitchen;
using KitchenData;
using Unity.Entities;
using UnityEngine;

namespace KitchenCustomDifficulty
{
    [UpdateAfter(typeof(UpdateQueuePatience))]
    internal class UpdateQueuePatiencePastCap : GameSystemBase
    {
        private EntityQuery Players;

        private EntityQuery Queuers;
        protected override void Initialise()
        {
            base.Initialise();
            Players = GetEntityQuery(typeof(CPlayer));
            Queuers = GetEntityQuery(typeof(CQueuePosition));
        }

        protected override void OnUpdate()
        {
            if (Has<SCheatNoPatienceDecrease>() ||
                HasStatus(RestaurantStatus.NoQueueReset) ||
                Main.PrefManager.Get<int>(Main.PHASE_PATIENCE_ENABLED_ID) == 0 ||
                Main.PrefManager.Get<int>(Main.PATIENCE_QUEUE_CAP_ID) == 0)
            {
                return;
            }
            float decayMultiplierPastCap = Mathf.Pow(1.1f, Queuers.CalculateEntityCount()) - 5f;
            if (decayMultiplierPastCap <= 0f)
            {
                return;
            }
            int player_count = Players.CalculateEntityCount();
            float playerMultiplier = DifficultyHelpers.PatiencePlayerCountModifier(player_count);
            float queuePatienceBoostBuffMutlipler = HasStatus(RestaurantStatus.HasQueuePatienceBoost) ? 0.75f : 1f;
            Entity sQueueMarkerEntity = GetSingletonEntity<SQueueMarker>();
            DynamicBuffer<CQueue> buffer = GetBuffer<CQueue>(sQueueMarkerEntity);
            bool isCustomerInQueue = false;
            Vector3 frontDoor = GetFrontDoor(get_external_tile: true);
            foreach (CQueue cQueue in buffer)
            {
                if (!Require<CPosition>(cQueue.Member, out CPosition cPosition) || !((cPosition - frontDoor).Chebyshev() < 3f))
                {
                    continue;
                }
                isCustomerInQueue = true;
                break;
            }
            CPatience cPatience = GetComponent<CPatience>(sQueueMarkerEntity);
            if (cPatience.Active && isCustomerInQueue)
            {
                float timePassed = base.Time.DeltaTime;
                float darknessMultiplier = ((!HasSingleton<SWeatherDarkness>()) ? 1 : 2);
                float weatherMultiplier;
                switch (GetWeather())
                {
                    case WeatherMode.None:
                        weatherMultiplier = 1f;
                        break;
                    case WeatherMode.Rain:
                        weatherMultiplier = 1.5f;
                        break;
                    case WeatherMode.Snow:
                        weatherMultiplier = 2f;
                        break;
                    default:
                        weatherMultiplier = 1f;
                        break;
                };
                darknessMultiplier *= weatherMultiplier;
                darknessMultiplier *= queuePatienceBoostBuffMutlipler;
                darknessMultiplier /= GameData.Main.Difficulty.QueuePatienceTime;
                cPatience.RemainingTime -= timePassed * darknessMultiplier * playerMultiplier * decayMultiplierPastCap;
            }
            SetComponent(sQueueMarkerEntity, cPatience);
        }
    }
}
