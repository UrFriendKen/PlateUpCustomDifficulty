using Kitchen;
using System.Reflection;
using UnityEngine;

namespace KitchenCustomDifficulty
{
    public class DeterminePlayerSpeed : Kitchen.DeterminePlayerSpeed
    {
        private float? basePlayerSpeed;
        private float? basePlayerMass;
        private float? mass;
        private static FieldInfo rigidbodyField;
        private float updateMassProgress = 0f;
        private float updateMassInterval = 0.5f;

        protected override void Initialise()
        {
            base.Initialise();
            rigidbodyField = typeof(PlayerView).GetField("Rigidbody", BindingFlags.NonPublic | BindingFlags.Instance);
        }

        protected override void OnUpdate()
        {
            float playerSpeedMultipler = Has<SIsNightTime>()? Main.PrefManager.Get<int>(Main.PLAYER_SPEED_PREP_ID) : Main.PrefManager.Get<int>(Main.PLAYER_SPEED_ID);

            if (playerSpeedMultipler != -2)
            {
                foreach (PlayerView playerView in UnityEngine.Object.FindObjectsOfType<PlayerView>())
                {
                    if (!basePlayerSpeed.HasValue)
                    {
                        basePlayerSpeed = playerView.Speed;
                    }

                    if (updateMassProgress > updateMassInterval || !mass.HasValue)
                    {
                        mass = ((Rigidbody)rigidbodyField.GetValue(playerView)).mass;
                        updateMassProgress = 0f;
                    }

                    if (!basePlayerMass.HasValue)
                    {
                        basePlayerMass = mass;
                    }

                    playerSpeedMultipler = (playerSpeedMultipler > -1) ? (playerSpeedMultipler / 100f) : 1f;
                    playerSpeedMultipler *= mass.Value / basePlayerMass.Value;
                    playerView.Speed = playerSpeedMultipler * basePlayerSpeed.Value;
                }
            }
            updateMassProgress += Time.DeltaTime;

            base.OnUpdate();
        }
    }
}
