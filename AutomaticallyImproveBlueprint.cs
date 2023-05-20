using Kitchen;
using Kitchen.Layouts;
using KitchenData;
using KitchenMods;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace KitchenCustomDifficulty
{
    [UpdateInGroup(typeof(TimeManagementGroup))]
    internal class AutomaticallyImproveBlueprint : RestaurantSystem
    {
        EntityQuery Desks;
        EntityQuery Discounts;

        protected override void Initialise()
        {
            base.Initialise();
            Desks = GetEntityQuery(new QueryHelper()
            .All(typeof(CDeskTarget), typeof(CModifyBlueprintStoreAfterDuration), typeof(CPosition))
            .None(typeof(CIsBroken)));
            Discounts = GetEntityQuery(typeof(CGrantsShopDiscount));
        }

        protected override void OnUpdate()
        {
            NativeArray<Entity> desks = Desks.ToEntityArray(Allocator.Temp);
            NativeArray<CDeskTarget> targets = Desks.ToComponentDataArray<CDeskTarget>(Allocator.Temp);
            NativeArray<CPosition> positions = Desks.ToComponentDataArray<CPosition>(Allocator.Temp);
            NativeArray<CModifyBlueprintStoreAfterDuration> improvements = Desks.ToComponentDataArray<CModifyBlueprintStoreAfterDuration>(Allocator.Temp);
            NativeArray<CGrantsShopDiscount> discounts = Discounts.ToComponentDataArray<CGrantsShopDiscount>(Allocator.Temp);

            bool perform = false;
            switch (Main.PrefSysManager.Get<int>(Main.DESK_AUTO_PERFORM_TIME_ID))
            {
                case 0:
                    perform = Has<SIsDayTime>();
                    break;
                case 1:
                    perform = (Require(out STime sTime) && sTime.TimeOfDayUnbounded >= 1f) || Has<SIsNightFirstUpdate>();
                    break;
                default:
                    break;
            }

            if (perform)
            {
                float blueprintCostMultiplier = 1f;
                foreach (CGrantsShopDiscount discount in discounts)
                {
                    blueprintCostMultiplier *= 1f - discount.Amount;
                }

                for (int i = 0; i < targets.Length; i++)
                {
                    Entity desk = desks[i];
                    CDeskTarget target = targets[i];
                    CPosition pos = positions[i];
                    CModifyBlueprintStoreAfterDuration improvement = improvements[i];

                    int num = Random.Range(0, LayoutHelpers.AllNearby.Count);
                    int room = GetRoom(pos);
                    for (int j = 0; j < LayoutHelpers.AllNearby.Count; j++)
                    {
                        LayoutPosition layoutPosition = LayoutHelpers.AllNearby[(j + num) % LayoutHelpers.AllNearby.Count];
                        Entity occupant = GetOccupant(pos.Position + (Vector3)layoutPosition);
                        int room2 = GetRoom(pos.Position + (Vector3)layoutPosition);
                        if (room == room2 && MeetsConditions(occupant, target))
                        {
                            bool performed = false;
                            if (!Require(occupant, out CBlueprintStore comp) || !comp.InUse ||
                                !this.Data.TryGet<Appliance>(comp.ApplianceID, out var output))
                            {
                                continue;
                            }
                            if (improvement.PerformUpgrade && output.HasUpgrades && !comp.HasBeenUpgraded && Main.PrefSysManager.Get<int>(Main.DESK_AUTO_RESEARCH_ID) == 1)
                            {
                                Appliance appliance = Kitchen.RandomExtensions.Random(output.Upgrades);
                                comp.Price = Mathf.CeilToInt((float)appliance.PurchaseCost * blueprintCostMultiplier);
                                if (comp.HasBeenMadeFree)
                                {
                                    comp.Price = Mathf.CeilToInt((float)comp.Price / 2f);
                                }
                                comp.ApplianceID = appliance.ID;
                                comp.BlueprintID = AssetReference.Blueprint;
                                comp.HasBeenUpgraded = true;
                                performed = true;
                            }
                            if (!comp.HasBeenCopied && improvement.PerformCopy && Main.PrefSysManager.Get<int>(Main.DESK_AUTO_COPY_ID) == 1)
                            {
                                comp.HasBeenCopied = true;
                                performed = true;
                            }
                            if (!comp.HasBeenMadeFree && improvement.MakeFree && Main.PrefSysManager.Get<int>(Main.DESK_AUTO_MAKE_FREE_ID) == 1)
                            {
                                comp.Price = Mathf.CeilToInt((float)comp.Price / 2f);
                                comp.HasBeenMadeFree = true;
                                performed = true;
                            }
                            if (performed && Require(occupant, out CCabinetModifier comp2) && comp2.DisablesDeskAfterImprovement)
                            {
                                Set<CIsBroken>(desk);
                                break;
                            }
                            Set(occupant, comp);

                            target.Target = default(Entity);
                        }
                    }
                }
            }
            desks.Dispose();
            targets.Dispose();
            improvements.Dispose();
            discounts.Dispose();
        }
        private bool MeetsConditions(Entity ent, CDeskTarget conditions)
        {
            if (!Require<CBlueprintStore>(ent, out CBlueprintStore comp))
            {
                return false;
            }
            if (!comp.InUse)
            {
                return false;
            }
            if (conditions.RequireMakeFree)
            {
                if (comp.Price <= 1)
                {
                    return false;
                }
                if (comp.HasBeenMadeFree)
                {
                    return false;
                }
            }
            if (conditions.RequireCopyable && comp.HasBeenCopied)
            {
                return false;
            }
            if (conditions.RequireUpgrade)
            {
                if (comp.HasBeenUpgraded)
                {
                    return false;
                }
                if (!base.Data.TryGet<Appliance>(comp.ApplianceID, out var output))
                {
                    return false;
                }
                if (!output.HasUpgrades)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
