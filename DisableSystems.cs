using Kitchen;
using KitchenMods;
using System;

namespace KitchenCustomDifficulty
{
    public class DisableSystems : FranchiseFirstFrameSystem, IModSystem
    {
        protected override void Initialise()
        {
            TrySetSystemEnabled<Kitchen.CheckGameOverFromLife>(false);
        }

        protected override void OnUpdate()
        {
        }

        private bool TrySetSystemEnabled<T>(bool isEnabled, bool logError = true) where T : GenericSystemBase
        {
            try
            {
                World.GetExistingSystem(typeof(T)).Enabled = isEnabled;
                Main.LogInfo($"{(isEnabled ? "Enabled" : "Disabled")} {typeof(T).FullName} system.");
                return true;
            }
            catch (NullReferenceException)
            {
                Main.LogInfo($"Failed to {(isEnabled ? "enable" : "disable")} {typeof(T).FullName} system. Are you in Multiplayer?");
                return false;
            }
        }
    }
}
