using Kitchen;
using KitchenData;
using Unity.Entities;

namespace KitchenCustomDifficulty
{
    [UpdateAfter(typeof(StartNewDay))]
    [UpdateInGroup(typeof(TimeManagementGroup))]
    internal class GroupSizeController : NightSystem
    {
        private struct SCacheGroupSize : IComponentData
        {
            public int Min;
            public int Max;
        }

        private static bool usingCustom = false;
        private static GroupSizeController instance;

        protected override void Initialise()
        {
            base.Initialise();
            instance = this;
        }

        protected override void OnUpdate()
        {
        }

        private void _SetCustomGroupSizes()
        {
            if (Require(out SKitchenParameters sKitchenParameters))
            {
                int minGroupSize = sKitchenParameters.Parameters.MinimumGroupSize;
                int maxGroupSize = sKitchenParameters.Parameters.MaximumGroupSize;
                int minGroupSizePref = Main.PrefSysManager.Get<int>(Main.CUSTOMERS_MIN_GROUP_SIZE_ID);
                int maxGroupSizePref = Main.PrefSysManager.Get<int>(Main.CUSTOMERS_MAX_GROUP_SIZE_ID);

                switch (minGroupSizePref)
                {
                    case -2:
                        break;
                    case -1:
                        minGroupSize = Main.DefaultValuesDict[Main.CUSTOMERS_MIN_GROUP_SIZE_ID];
                        break;
                    default:
                        minGroupSize = minGroupSizePref;
                        break;
                }

                switch (maxGroupSizePref)
                {
                    case -2:
                        break;
                    case -1:
                        maxGroupSize = Main.DefaultValuesDict[Main.CUSTOMERS_MAX_GROUP_SIZE_ID];
                        break;
                    default:
                        maxGroupSize = maxGroupSizePref;
                        break;
                }

                if (maxGroupSize < minGroupSize)
                    maxGroupSize = minGroupSize;

                sKitchenParameters.Parameters.MinimumGroupSize = minGroupSize;
                sKitchenParameters.Parameters.MaximumGroupSize = maxGroupSize;
                Set(sKitchenParameters);
            }
        }

        private void _ResetKitchenParameters()
        {
            if (Has<SKitchenParameters>())
            {
                SKitchenParameters sKitchenParameters = GetOrDefault<SKitchenParameters>();
                if (TryGetSingleton(out SCacheGroupSize cache))
                {
                    sKitchenParameters.Parameters.MinimumGroupSize = cache.Min;
                    sKitchenParameters.Parameters.MaximumGroupSize = cache.Max;
                    Set(sKitchenParameters);
                }
            }
        }

        private void _CacheKitchenParameters()
        {
            if (!usingCustom)
            {
                SKitchenParameters sKitchenParameters = GetOrDefault<SKitchenParameters>();
                Set(new SCacheGroupSize
                {
                    Min = sKitchenParameters.Parameters.MinimumGroupSize,
                    Max = sKitchenParameters.Parameters.MaximumGroupSize
                });
            }
        }

        private void _UpdateCacheOnParameterChange(ParameterEffect parameterEffect)
        {
            SCacheGroupSize cache = GetOrDefault<SCacheGroupSize>();
            cache.Min += parameterEffect.Parameters.MinimumGroupSize;
            cache.Max += parameterEffect.Parameters.MaximumGroupSize;
            Set(cache);
        }

        public static void UseCustomKitchenParameters()
        {
            instance._CacheKitchenParameters();
            usingCustom = true;
            instance._SetCustomGroupSizes();
        }

        public static void ResetKitchenParameters()
        {
            instance._ResetKitchenParameters();
            usingCustom = false;
        }

        public static void HandleVanillaParameterChange(ParameterEffect parameterEffect)
        {
            instance._UpdateCacheOnParameterChange(parameterEffect);
        }
    }
}
