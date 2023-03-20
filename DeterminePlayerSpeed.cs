using Kitchen;
using System.Reflection;
using UnityEngine;

namespace KitchenCustomDifficulty
{
    public class DeterminePlayerSpeed : Kitchen.DeterminePlayerSpeed
    {
        private float? basePlayerSpeed;
        private float? basePlayerMass;
        private static FieldInfo rigidbodyField;

        protected override void Initialise()
        {
            base.Initialise();
            rigidbodyField = typeof(PlayerView).GetField("Rigidbody", BindingFlags.NonPublic | BindingFlags.Instance);
        }

        protected override void OnUpdate()
        {
            float playerSpeedMultiplier = Has<SIsNightTime>()? Main.PrefManager.Get<int>(Main.PLAYER_SPEED_PREP_ID) : Main.PrefManager.Get<int>(Main.PLAYER_SPEED_ID);
            if (playerSpeedMultiplier != -2)
            {
                playerSpeedMultiplier = (playerSpeedMultiplier > -1) ? (playerSpeedMultiplier / 100f) : 1f;
                foreach (PlayerView playerView in UnityEngine.Object.FindObjectsOfType<PlayerView>())
                {
                    if (!basePlayerSpeed.HasValue)
                    {
                        basePlayerSpeed = playerView.Speed;
                    }

                    float mass = ((Rigidbody)rigidbodyField.GetValue(playerView)).mass;
                    if (!basePlayerMass.HasValue)
                    {
                        basePlayerMass = mass;
                    }

                    playerView.Speed = playerSpeedMultiplier * mass / basePlayerMass.Value * basePlayerSpeed.Value;
                }
            }

            base.OnUpdate();
        }
    }
}
