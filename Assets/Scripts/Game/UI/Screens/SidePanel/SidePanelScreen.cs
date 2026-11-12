using Scrapwalker.Game.UI.Widgets;
using Scrapwalker.Game.Wording.Words;
using UnityEngine.UIElements;

namespace Scrapwalker.Game.UI.Screens.SidePanel
{
    public sealed class SidePanelScreen : UIScreen
    {
        private readonly VisualElement _panel;
        private readonly SideStockPage _stock;
        private readonly SideBotsPage _bots;
        private readonly SideSettingsPage _settings;
        private readonly ButtonChoice<SidePage> _tabs;
        private readonly ShownValue<bool> _openShown = new ShownValue<bool>();
        private readonly ShownValue<SidePage> _pageShown = new ShownValue<SidePage>();

        public SidePanelScreen(VisualElement root) : base(root)
        {
            _panel = Q("side-panel");
            _tabs = new ButtonChoice<SidePage>("tab--selected", page => UIState.OpenSide(page));
            root.WriteLabel("tab-stock-label", SideWords.TabStock);
            root.WriteLabel("tab-bots-label", SideWords.TabBots);
            root.WriteLabel("tab-settings-label", SideWords.TabSettings);
            _stock = AddPart(new SideStockPage(Q("page-stock"), Root.TakeTemplate("stock-cell-template")));
            _bots = AddPart(new SideBotsPage(Q("page-bots"), Root.TakeTemplate("bot-row-template")));
            _settings = AddPart(new SideSettingsPage(Q("page-settings")));
            AddTab(SidePage.Stock, "tab-stock", _stock);
            AddTab(SidePage.Bots, "tab-bots", _bots);
            AddTab(SidePage.Settings, "tab-settings", _settings);
        }

        protected override void OnBind()
        {
            _openShown.Reset();
            _pageShown.Reset();
        }

        public void Refresh()
        {
            bool isOpen = UIState.IsSideOpen;
            if (_openShown.Change(isOpen))
            {
                _panel.Display(isOpen);
            }

            if (!isOpen)
            {
                return;
            }

            SidePage page = UIState.SidePage;
            if (_pageShown.Change(page))
            {
                _tabs.Select(page);
            }

            switch (page)
            {
                case SidePage.Stock:
                    _stock.Refresh();
                    break;
                case SidePage.Bots:
                    _bots.Refresh();
                    break;
                default:
                    break;
            }
        }

        private void AddTab(SidePage page, string tabName, UIScreen screen)
        {
            _tabs.Add(page, Q<Button>(tabName), screen.Root.Display);
        }
    }
}
