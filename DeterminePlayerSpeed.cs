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
            int playerSpeedMultipler = Has<SIsNightTime>()? Main.PrefManager.Get<int>(Main.PLAYER_SPEED_PREP_ID) : Main.PrefManager.Get<int>(Main.PLAYER_SPEED_ID);

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
            base.OnUpdate();
        }
    }
}
