using Scrapwalker.Core.Root;
using Scrapwalker.Core.Sessions;
using Scrapwalker.Game.Definitions;

namespace Scrapwalker.Game.UI
{
    public sealed class UIContext
    {
        public UIContext(Session session, UIState ui, IAppHost app, GameData data)
        {
            Session = session;
            World = session.World;
            UI = ui;
            App = app;
            Data = data;
        }

        public Session Session { get; }
        public World World { get; }
        public UIState UI { get; }
        public IAppHost App { get; }
        public GameData Data { get; }
    }
}
