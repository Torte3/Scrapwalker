namespace Scrapwalker.Game.Wording.Words
{
    public static class FilterWords
    {
        public const string RangeTitle = "使う品質の範囲";
        public const string OrderTitle = "品質の優先度";
        public const string LowFirst = "低い方から使う";
        public const string HighFirst = "高い方から使う";

        public static string Range(string low, string high)
        {
            return $"{low}〜{high}";
        }
    }
}
