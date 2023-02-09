using Kitchen;
using KitchenMods;
using Unity.Entities;

namespace KitchenCustomDifficulty
{
    [UpdateBefore(typeof(Kitchen.CheckGameOverFromLife))]
    public class CheckGameOverFromLife : Kitchen.CheckGameOverFromLife
    {
        private struct CRestartChanceOnLossIssued : IModComponent { }

        protected override void Initialise()
        {
            base.Initialise();
        }

        protected override void OnUpdate()
        {
            if (HasSingleton<SGameOver>())
            {
                Clear<CRestartChanceOnLossIssued>();
                return;
            }

            SKitchenStatus kitchenStatus = GetSingleton<SKitchenStatus>();
            if (kitchenStatus.RemainingLives <= 0 && !HasSingleton<CRestartChanceOnLossIssued>())
            {
                int isRestartChanceEnabled = Main.RestartOnLossPreference.Load(Main.MOD_GUID);
                if (!HasSingleton<SGameOver>() && !Has<SPracticeMode>() && isRestartChanceEnabled == 1)
                {
                    base.World.Add(new COfferRestartDay
                    {
                        Reason = LossReason.Patience
                    });
                    SetSingleton(new SKitchenStatus
                    {
                        RemainingLives = kitchenStatus.TotalLives,
                        TotalLives = kitchenStatus.TotalLives
                    });
                    base.World.Add(new CRestartChanceOnLossIssued { });
                }
                else
                {
                    base.OnUpdate();
                }
            }
        }
    }
}
