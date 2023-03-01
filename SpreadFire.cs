using Kitchen;
using Kitchen.Layouts;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace KitchenCustomDifficulty
{
    internal class SpreadFire : GameSystemBase
    {
        EntityQuery Players;
        EntityQuery AppliancesOnFire;
        protected override void Initialise()
        {
            base.Initialise();
            Players = GetEntityQuery(typeof(CPlayer));
            AppliancesOnFire = GetEntityQuery(typeof(CIsOnFire), typeof(CPosition));
        }

        protected override void OnUpdate()
        {
            NativeArray<Entity> appliancesOnFire = AppliancesOnFire.ToEntityArray(Allocator.Temp);
            NativeArray<CPosition> appliancesOnFirePosition = AppliancesOnFire.ToComponentDataArray<CPosition>(Allocator.Temp);

            float dt = Time.DeltaTime;
            float player_factor = DifficultyHelpers.FireSpreadModifier(Players.CalculateEntityCount());

            for (int i = 0; i < appliancesOnFire.Length; i++)
            {
                Entity appliance = appliancesOnFire[i];
                CPosition pos = appliancesOnFirePosition[i];

                int room = GetRoom(pos);
                foreach (LayoutPosition item in LayoutHelpers.AllNearby)
                {
                    Vector3 position = (Vector3)item + (Vector3)pos;
                    Entity primaryOccupant = GetPrimaryOccupant(position);
                    if (EntityManager.HasComponent<CAppliance>(primaryOccupant) && EntityManager.HasComponent<CIsInteractive>(primaryOccupant) &&
                        !EntityManager.HasComponent<CFireImmune>(primaryOccupant) && !EntityManager.HasComponent<CIsOnFire>(primaryOccupant))
                    {
                        if (GetRoom(position) == room || Main.PrefManager.Get<int>(Main.FIRE_SPREAD_THROUGH_WALLS_ID) == 1)
                        {
                            double num = (EntityManager.HasComponent<CHighlyFlammable>(primaryOccupant) ? 0.1 : 0.02);
                            if ((double)Random.value < num * (double)dt * (double)player_factor)
                            {
                                EntityManager.AddComponent<CIsOnFire>(primaryOccupant);
                            }
                        }
                    }
                }
            }
        }
    }
}

