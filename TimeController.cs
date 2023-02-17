using Kitchen;

namespace KitchenCustomDifficulty
{
    internal class TimeController : DaySystem
    {
        public static float CurrentDayLength { get; private set; }

        protected override void Initialise()
        {
            base.Initialise();
        }

        protected override void OnUpdate()
        {
            if (!Has<STime>() || !Has<SDay>())
            {
                return;
            }
            STime sTime = GetOrDefault<STime>();
            int day = GetOrDefault<SDay>().Day;
            sTime.DayLength = ProgressionHelpers.GetDayLength(day);
            Set(sTime);
        }
    }
}
