using Kitchen;
using KitchenData;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace KitchenCustomDifficulty
{
    internal class AutomaticallyImproveBlueprint : DaySystem
    {
        EntityQuery Desks;
        EntityQuery Discounts;

        protected override void Initialise()
        {
            base.Initialise();
            Desks = GetEntityQuery(new QueryHelper()
            .All(typeof(CDeskTarget), typeof(CModifyBlueprintStoreAfterDuration))
            .None(typeof(CIsBroken)));
            Discounts = GetEntityQuery(typeof(CGrantsShopDiscount));
        }

        protected override void OnUpdate()
        {
            NativeArray<Entity> desks = Desks.ToEntityArray(Allocator.Temp);
            NativeArray<CDeskTarget> targets = Desks.ToComponentDataArray<CDeskTarget>(Allocator.Temp);
            NativeArray<CModifyBlueprintStoreAfterDuration> improvements = Desks.ToComponentDataArray<CModifyBlueprintStoreAfterDuration>(Allocator.Temp);

            NativeArray<CGrantsShopDiscount> discounts = Discounts.ToComponentDataArray<CGrantsShopDiscount>(Allocator.Temp);
            float blueprintCostMultiplier = 1f;
            foreach (CGrantsShopDiscount discount in discounts)
            {
                blueprintCostMultiplier *= 1f - discount.Amount;
            }


            for (int i = 0; i < targets.Length; i++)
            {
                Entity desk = desks[i];
                CDeskTarget target = targets[i];
                CModifyBlueprintStoreAfterDuration improvement = improvements[i];

                bool performed = false;
                if (!Require(target.Target, out CBlueprintStore comp) || !comp.InUse ||
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
                if (performed && Require(target.Target, out CCabinetModifier comp2) && comp2.DisablesDeskAfterImprovement)
                {
                    Set<CIsBroken>(desk);
                }
                Set(target.Target, comp);

                target.Target = default(Entity);
            }

            desks.Dispose();
            targets.Dispose();
            improvements.Dispose();
            discounts.Dispose();
        }
    }
}
