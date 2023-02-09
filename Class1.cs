using Kitchen;
using KitchenData;
using KitchenMods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Collections;
using Unity.Entities;

namespace KitchenCustomDifficulty
{
    public struct CAdjustableProcessSpeedModifier : IEffectType, IModComponent
    {
        public int Process;

        public float Speed;
    }

    [UpdateInGroup(typeof(DetermineEffectsGroup))]
    public class ApplyCustomDifficultySpeedModifier : GameEffectSystemBase
    {
        private EntityQuery EffectTargets;

        protected override void Initialise()
        {
            base.Initialise();

            EffectTargets = GetEntityQuery(
                typeof(CAffectedBy),
                typeof(CPosition),
                typeof(CAppliance)
            );
        }

        protected override void OnUpdate()
        {
            var targets = EffectTargets.ToEntityArray(Allocator.Temp);

            foreach (var target in targets)
            {
                if (!RequireBuffer(target, out DynamicBuffer<CAffectedBy> affected_by)) continue;

                foreach (var effector in affected_by)
                {
                    // check the effect is active and is the correct type
                    if (!Require(effector, out CAppliesEffect eff) || !eff.IsActive) continue;
                    if (!Require(effector, out CAdjustableProcessSpeedModifier adjustable_process_speed_modifier)) continue;

                    // now apply the effect
                    Set(target, adjustable_process_speed_modifier);
                }
            }
        }
    }
}
