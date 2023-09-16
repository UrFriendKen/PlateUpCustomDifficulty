using Kitchen;
using KitchenMods;

namespace KitchenCustomDifficulty
{
    public class HandleCheats : RestaurantSystem, IModSystem
    {
        protected override void Initialise()
        {
            base.Initialise();
        }

        protected override void OnUpdate()
        {
            switch (Main.PrefSysManager.Get<int>(Main.CHEAT_NO_LOSING_ID))
            {
                case 0:
                    if (Has<SCheatNoLosing>())
                        Clear<SCheatNoLosing>();
                    break;
                case 1:
                    if (!Has<SCheatNoLosing>())
                        Set<SCheatNoLosing>();
                    break;
            }
            int instantProcesses = Main.PrefSysManager.Get<int>(Main.CHEAT_INSTANT_PROCESSES_ID);
            if (instantProcesses < 1)
            {
                if (Has<SCheatInstantProcesses>())
                    Clear<SCheatInstantProcesses>();
                if (Has<SCheatNoBadProcesses>())
                    Clear<SCheatNoBadProcesses>();
            }
            else if (instantProcesses < 2)
            {
                if (!Has<SCheatInstantProcesses>())
                    Set<SCheatInstantProcesses>();
                if (!Has<SCheatNoBadProcesses>())
                    Set<SCheatNoBadProcesses>();
            }
            else
            {
                if (!Has<SCheatInstantProcesses>())
                    Set<SCheatInstantProcesses>();
                if (Has<SCheatNoBadProcesses>())
                    Clear<SCheatNoBadProcesses>();
            }
        }
    }
}
