using Kitchen;
using KitchenMods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Entities;

namespace KitchenCustomDifficulty
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    [UpdateAfter(typeof(UpdateTime))]
    public class ScaleTime : GenericSystemBase, IModSystem
    {
        protected override void OnUpdate()
        {
            if (!TryGetSingleton(out SGameTime gameTime))
            {
                return;
            }

            float gameSpeed = Main.PrefSysManager.Get<int>(Main.GAME_SPEED_ID);
            if (gameSpeed == -1)
                gameSpeed = Main.DefaultValuesDict[Main.GAME_SPEED_ID];

            gameTime.GameSpeed = gameSpeed/100f;
            SetSingleton(gameTime);
        }
    }
}
