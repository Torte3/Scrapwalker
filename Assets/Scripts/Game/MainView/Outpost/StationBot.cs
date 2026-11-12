using Scrapwalker.Game.MainView.Bots;

namespace Scrapwalker.Game.MainView.Outpost
{
    public readonly struct StationBot
    {
        public StationBot(BotBody body)
        {
            Body = body;
        }

        public BotBody Body { get; }
    }
}
