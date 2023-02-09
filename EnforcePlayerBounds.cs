using Kitchen;

namespace KitchenCustomDifficulty
{
    public class EnforcePlayerBounds : Kitchen.EnforcePlayerBounds
    {
        protected override void Initialise()
        {
            base.Initialise();
        }

        protected override void OnUpdate()
        {
            bool allowOutOfBounds = Main.PlayerOutOfBoundsPreference.Load(Main.MOD_GUID) == 1;


            if (allowOutOfBounds)
            {
                if (Has<SIsNightTime>() && Main.PlayerCollisionPrepPreference.Load(Main.MOD_GUID) > 2)
                {
                    return;
                }
                if (Has<SIsDayTime>() && Main.PlayerCollisionPreference.Load(Main.MOD_GUID) > 2)
                {
                    return;
                }
            }
            base.OnUpdate();
        }
    }
}
