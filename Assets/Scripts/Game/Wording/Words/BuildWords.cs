namespace Scrapwalker.Game.Wording.Words
{
    public static class BuildWords
    {
        public const string NoCost = "資材なし";
        public const string LimitLabel = "建設数";

        public static string Tab(string category, int count)
        {
            return $"{category} {count}";
        }

        public static string Page(string page, string pages)
        {
            return $"{page} / {pages}";
        }

        public static string Limit(int count, int limit)
        {
            return $"{count}/{limit}";
        }
    }
}
