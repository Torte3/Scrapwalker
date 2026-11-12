namespace Scrapwalker.Game.Wording.Words
{
    public static class WorldWords
    {
        public const string OverLimit = "建築上限超過";
        public const string Building = "建設中";
        public const string Rebuilding = "修理中";
        public const string Reassembling = "移設中";
        public const string Upgrading = "改造中";
        public const string Dismantling = "解体中";
        public const string ClickPlace = "クリックで配置";
        public const string ClickRelocate = "クリックで移設";

        public static string RelocateTo(string name)
        {
            return $"移設先：{name}";
        }
    }
}
