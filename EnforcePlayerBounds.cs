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
            bool allowOutOfBounds = Main.PrefManager.Get<int>(Main.PLAYER_OUT_OF_BOUNDS_ID) == 1;


            if (allowOutOfBounds)
            {
                if (Has<SIsNightTime>() && Main.PrefManager.Get<int>(Main.PLAYER_COLLISION_PREP_ID) > 2)
                {
                    return;
                }
                if (Has<SIsDayTime>() && Main.PrefManager.Get<int>(Main.PLAYER_COLLISION_ID) > 2)
                {
                    return;
                }
            }
            base.OnUpdate();
        }
    }
}
