namespace Scrapwalker.Game.Wording.Words
{
    public static class RouteWords
    {
        public const string Title = "行き先";
        public const string Cleared = "完走済み";
        public const string Repeat = "エンドレス";
        public const string Land = "ここに降りる";
        public const string Heading = "向かっている";
        public const string Unreachable = "届かない";
        public const string NotMoving = "歩行中だけ選べる";
        public const string Decided = "行き先は決まっている";
        public const string MainItemsSeparator = "・";

        public static string MainItems(string items)
        {
            return $"主な資材：{items}";
        }
    }
}
