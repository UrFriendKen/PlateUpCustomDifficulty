﻿using Kitchen;
using Kitchen.Layouts;
using KitchenData;
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
            .All(typeof(CDeskTarget), typeof(CPosition))
            .None(typeof(CIsBroken)));
            Discounts = GetEntityQuery(typeof(CGrantsShopDiscount));
        }

        protected override void OnUpdate()
        {
            NativeArray<Entity> desks = Desks.ToEntityArray(Allocator.Temp);
            NativeArray<CDeskTarget> targets = Desks.ToComponentDataArray<CDeskTarget>(Allocator.Temp);
            NativeArray<CPosition> positions = Desks.ToComponentDataArray<CPosition>(Allocator.Temp);
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

                    CModifyBlueprintStoreAfterDuration improvement;
                    bool isImprove = Require(desk, out improvement);

                    bool isEnchant = Has<CEnchantBlueprintAfterDuration>(desk);


                    int room = GetRoom(pos);
                    foreach (LayoutPosition layoutPosition in Kitchen.RandomExtensions.Shuffle(LayoutHelpers.AllNearby))
                    {
                        Entity occupant = GetOccupant(pos.Position + (Vector3)layoutPosition);
                        int room2 = GetRoom(pos.Position + (Vector3)layoutPosition);
                        if (room == room2 && MeetsConditions(occupant, target, isEnchant))
                        {
                            bool performed = false;
                            if (!Require(occupant, out CBlueprintStore comp) || !comp.InUse ||
                                !this.Data.TryGet<Appliance>(comp.ApplianceID, out var output))
                            {
                                continue;
                            }
                            if (isImprove)
                            {
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
                            }
                            if (isEnchant)
                            {
                                if (output.HasEnchantments && !comp.HasBeenUpgraded && Main.PrefSysManager.Get<int>(Main.DESK_AUTO_ENCHANT_ID) == 1)
                                {
                                    Appliance appliance = Kitchen.RandomExtensions.Random(output.Enchantments);
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
                            }
                            Set(occupant, comp);
                            target.Target = default(Entity);
                            if (performed)
                            {
                                Set(desk, target);
                                if (Require(occupant, out CCabinetModifier comp2) && comp2.DisablesDeskAfterImprovement)
                                {
                                    Set<CIsBroken>(desk);
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            desks.Dispose();
            targets.Dispose();
            positions.Dispose();
            discounts.Dispose();
        }
        private bool MeetsConditions(Entity ent, CDeskTarget conditions, bool needs_enchant = false)
        {
            if (!Require<CBlueprintStore>(ent, out CBlueprintStore comp))
            {
                return false;
            }
            if (!comp.InUse)
            {
                return false;
            }
            if (needs_enchant)
            {
                if (comp.HasBeenUpgraded)
                {
                    return false;
                }
                if (!base.Data.TryGet<Appliance>(comp.ApplianceID, out var output))
                {
                    return false;
                }
                if (!output.HasEnchantments)
                {
                    return false;
                }
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
