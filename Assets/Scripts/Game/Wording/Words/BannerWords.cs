namespace Scrapwalker.Game.Wording.Words
{
    public static class BannerWords
    {
        public const string Cleared = "ALL WAVES CLEAR";
        public const string ClearedLabel = "COMPLETE";
        public const string Launch = "LAUNCH SEQUENCE";
        public const string LaunchLabel = "FINAL WAVE  ―  CHARGING";

        public static string Warning(int number)
        {
            return $"WAVE {number} START";
        }

        public static string WarningLabel(int number, int count)
        {
            return $"WARNING  ―  WAVE {number} / {count}";
        }

        public static string WaveClear(int number)
        {
            return $"WAVE {number} CLEAR";
        }

        public static string WaveClearLabel(int number, int count)
        {
            return $"WAVE {number} / {count}";
        }
    }
}
