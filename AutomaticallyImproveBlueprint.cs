using Kitchen;
using System.Linq;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace KitchenCustomDifficulty
{
    internal class AutomaticallyImproveBlueprint : DaySystem
    {
        EntityQuery Desks;

        protected override void Initialise()
        {
            base.Initialise();
            Desks = GetEntityQuery(new QueryHelper()
            .All(typeof(CDeskTarget), typeof(CModifyBlueprintStoreAfterDuration)));
        }

        protected override void OnUpdate()
        {
            NativeArray<Entity> desks = Desks.ToEntityArray(Allocator.Temp);
            NativeArray<CDeskTarget> targets = Desks.ToComponentDataArray<CDeskTarget>(Allocator.Temp);
            NativeArray<CModifyBlueprintStoreAfterDuration> improvements = Desks.ToComponentDataArray<CModifyBlueprintStoreAfterDuration>(Allocator.Temp);

            Main.LogInfo(desks.Count());
            for (int i = 0; i < targets.Length; i++)
            {
                Entity desk = desks[i];
                CDeskTarget target = targets[i];
                CModifyBlueprintStoreAfterDuration improvement = improvements[i];

                if (!Require(target.Target, out CBlueprintStore comp) || !comp.InUse)
                {
                    continue;
                }
                if (improvement.PerformCopy && Main.PrefManager.Get<int>(Main.DESK_AUTO_COPY_ID) == 1)
                {
                    comp.HasBeenCopied = true;
                }
                if (improvement.MakeFree && Main.PrefManager.Get<int>(Main.DESK_AUTO_MAKE_FREE_ID) == 1)
                {
                    comp.Price = Mathf.CeilToInt((float)comp.Price / 2f);
                    comp.HasBeenMadeFree = true;
                }
                if (Require(target.Target, out CCabinetModifier comp2) && comp2.DisablesDeskAfterImprovement)
                {
                    Set<CIsBroken>(desk);
                }
                Set(target.Target, comp);

                target.Target = default(Entity);
            }

            desks.Dispose();
            targets.Dispose();
            improvements.Dispose();
        }
    }
}
