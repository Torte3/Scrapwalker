namespace Scrapwalker.Game.Wording.Words
{
    public static class StockWords
    {
        public const string Separator = "・";

        public static string Quality(string tier, int count)
        {
            return $"{tier} {count}";
        }
    }
}
