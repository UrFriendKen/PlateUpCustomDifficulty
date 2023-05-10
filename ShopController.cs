using Kitchen;
using KitchenMods;
using Unity.Entities;

namespace KitchenCustomDifficulty
{
    public struct SGlobalShopDiscount : IComponentData, IModComponent { }

    public class ShopController : RestaurantSystem
    {
        protected override void OnUpdate()
        {
            if (!TryGetSingletonEntity<SGlobalShopDiscount>(out Entity globalShopDiscountSingleton))
            {
                globalShopDiscountSingleton = EntityManager.CreateEntity(typeof(SGlobalShopDiscount), typeof(CGrantsShopDiscount));
            }
            Set(globalShopDiscountSingleton, new CGrantsShopDiscount()
            {
                Amount = GetGlobalDiscountAmount()
            });
        }
        
        private float GetGlobalDiscountAmount()
        {
            int prefVal = Main.PrefSysManager.Get<int>(Main.SHOP_COST_MULTIPLIER);
            if (prefVal == -1)
            {
                prefVal = Main.DefaultValuesDict[Main.SHOP_COST_MULTIPLIER];
            }
            return (100 - prefVal) / 100f;
        }
    }
}
