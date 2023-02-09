using Kitchen;

namespace KitchenCustomDifficulty
{
    public class DeterminePlayerSpeed : Kitchen.DeterminePlayerSpeed
    {
        private static float? basePlayerSpeed;
        protected override void Initialise()
        {
            base.Initialise();
        }

        protected override void OnUpdate()
        {

            if (Has<SIsNightTime>())
            {
                int playerSpeedPrepMultipler = Main.PlayerSpeedPrepPreference.Load(Main.MOD_GUID);

                if (playerSpeedPrepMultipler != -2)
                {
                    foreach (PlayerView playerView in UnityEngine.Object.FindObjectsOfType<PlayerView>())
                    {
                        if (!basePlayerSpeed.HasValue)
                        {
                            basePlayerSpeed = playerView.Speed;
                        }

                        playerView.Speed = (playerSpeedPrepMultipler > -1) ? (basePlayerSpeed.Value * (playerSpeedPrepMultipler / 100f)) : basePlayerSpeed.Value;
                    }
                }
            }
            else
            {
                int playerSpeedMultipler = Main.PlayerSpeedPreference.Load(Main.MOD_GUID);

                if (playerSpeedMultipler != -2)
                {
                    foreach (PlayerView playerView in UnityEngine.Object.FindObjectsOfType<PlayerView>())
                    {
                        if (!basePlayerSpeed.HasValue)
                        {
                            basePlayerSpeed = playerView.Speed;
                        }

                        playerView.Speed = (playerSpeedMultipler > -1) ? (basePlayerSpeed.Value * (playerSpeedMultipler / 100f)) : basePlayerSpeed.Value;
                    }
                }
            }

            base.OnUpdate();
        }
    }
}
