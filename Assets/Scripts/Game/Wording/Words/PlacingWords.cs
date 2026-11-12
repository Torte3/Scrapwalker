namespace Scrapwalker.Game.Wording.Words
{
    public static class PlacingWords
    {
        public const string Back = "← 戻る（Esc）";
        public const string Move = "移設";
        public const string Build = "建設";
        public const string Need = "必要な資材";
        public const string Result = "できあがる設備";
        public const string Quality = "設備の品質";
        public const string NoQuality = "—";
        public const string UseQuality = "使う品質";

        public static string Head(string category, string size)
        {
            return $"{category}・{size}";
        }
    }
}
