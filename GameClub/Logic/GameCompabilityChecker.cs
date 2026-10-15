namespace GameClub.Logic
{
    public static class GameCompatibilityChecker
    {
        public static bool CanRun(int totalPerformanceLevel, int minPerformanceLevel)
        {
            return totalPerformanceLevel >= minPerformanceLevel;
        }
    }
}