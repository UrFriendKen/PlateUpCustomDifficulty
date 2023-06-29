using Kitchen;
using KitchenMods;
using Unity.Entities;

namespace KitchenCustomDifficulty
{
    public class PatchController : GenericSystemBase, IModSystem
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
    }
}
