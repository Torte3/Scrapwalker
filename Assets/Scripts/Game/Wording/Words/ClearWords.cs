namespace Scrapwalker.Game.Wording.Words
{
    public static class ClearWords
    {
        public const string Title = "脱出";
        public const string Time = "クリアタイム";
        public const string Ruins = "完走した廃墟";
        public const string Research = "研究";
        public const string Rooms = "拠点の部屋数";
        public const string Bots = "作業ボット";
        public const string Kills = "倒した敵";
        public const string Materials = "集めた資材";
        public const string Waves = "凌いだウェーブ";
        public const string Next = "次へ";

        public static string Total(string total)
        {
            return $"/ {total}";
        }
    }
}
