using System.Globalization;

namespace Scrapwalker.Game.Wording
{
    public static class Texts
    {
        public static string Times(int count)
        {
            return Times(count.ToString());
        }

        public static string Times(string count)
        {
            return "×" + count;
        }

        public static string Seconds(float seconds)
        {
            return seconds.ToString("0.##", CultureInfo.InvariantCulture);
        }

        public static string Decimal(float value)
        {
            return value.ToString("0.#", CultureInfo.InvariantCulture);
        }

        public static string Minutes(int seconds)
        {
            return Clock(seconds / 60, seconds % 60);
        }

        public static string Hours(int seconds)
        {
            return Clock(seconds / 3600, seconds / 60 % 60, seconds % 60);
        }

        public static string Monospace(object value)
        {
            return "<mspace=0.6em>" + value + "</mspace>";
        }

        private static string Clock(int minutes, int seconds)
        {
            return $"{minutes}:{seconds:00}";
        }

        private static string Clock(int hours, int minutes, int seconds)
        {
            return $"{hours}:{minutes:00}:{seconds:00}";
        }
    }
}
