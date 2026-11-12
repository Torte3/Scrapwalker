using Scrapwalker.Core.Outpost.Bots;

namespace Scrapwalker.Game.Wording
{
    public static class BotText
    {
        public static string Label(this Bot bot)
        {
            int id = bot.ID;
            return $"B-{id:00}";
        }
    }
}
