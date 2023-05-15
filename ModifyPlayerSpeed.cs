using Kitchen;
using Unity.Collections;
using Unity.Entities;

namespace KitchenCustomDifficulty
{
    [UpdateAfter(typeof(DeterminePlayerSpeed))]
    public class ModifyPlayerSpeed : GenericSystemBase
    {
        EntityQuery Players;
        protected override void Initialise()
        {
            base.Initialise();
            Players = GetEntityQuery(new QueryHelper()
                .All(typeof(CPlayer)));
        }

        protected override void OnUpdate()
        {
            NativeArray<Entity> entities = Players.ToEntityArray(Allocator.Temp);
            NativeArray<CPlayer> players = Players.ToComponentDataArray<CPlayer>(Allocator.Temp);

            float playerSpeedMultiplier = Has<SIsNightTime>()? Main.PrefSysManager.Get<int>(Main.PLAYER_SPEED_PREP_ID) : Main.PrefSysManager.Get<int>(Main.PLAYER_SPEED_ID);
            if (playerSpeedMultiplier != -2)
            {
                playerSpeedMultiplier = (playerSpeedMultiplier > -1) && GameInfo.CurrentScene == SceneType.Kitchen ? (playerSpeedMultiplier / 100f) : 1f;

                for (int i = 0; i < entities.Length; i++)
                {
                    Entity entity = entities[i];
                    CPlayer player = players[i];

                    player.Speed *= playerSpeedMultiplier;
                    Set(entity, player);
                }
            }

            entities.Dispose();
            players.Dispose();
        }
    }
}
