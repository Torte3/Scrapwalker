using System.Collections.Generic;
using Scrapwalker.Core.Sessions;
using Scrapwalker.Core.Travel;
using Scrapwalker.Game.Look;
using Scrapwalker.Game.UI.Screens.BuildList;
using Scrapwalker.Game.UI.Screens.Clear;
using Scrapwalker.Game.UI.Screens.EdgeHint;
using Scrapwalker.Game.UI.Screens.FacilityMenu;
using Scrapwalker.Game.UI.Screens.FacilityPanel;
using Scrapwalker.Game.UI.Screens.Fade;
using Scrapwalker.Game.UI.Screens.Operations;
using Scrapwalker.Game.UI.Screens.Placing;
using Scrapwalker.Game.UI.Screens.Route;
using Scrapwalker.Game.UI.Screens.Save;
using Scrapwalker.Game.UI.Screens.SidePanel;
using Scrapwalker.Game.UI.Screens.Ticker;
using Scrapwalker.Game.UI.Screens.Title;
using Scrapwalker.Game.UI.Screens.TopBar;
using Scrapwalker.Game.UI.Screens.Tutorial;
using Scrapwalker.Game.UI.Screens.WaveBanner;
using Scrapwalker.Game.UI.Widgets;
using UnityEngine;
using UnityEngine.UIElements;

namespace Scrapwalker.Game.UI
{
    [RequireComponent(typeof(UIDocument))]
    public sealed class HUDView : MonoBehaviour
    {
        private readonly List<UIScreen> _screens = new List<UIScreen>();
        private readonly List<Rect> _menuRects = new List<Rect>();
        private readonly List<Rect> _timeRects = new List<Rect>();
        private readonly List<Button> _controls = new List<Button>();
        private readonly List<Rect> _controlRects = new List<Rect>();
        private UIContext _context;
        private HUDTutorialTargets _targets;
        private WaveBannerScreen _waveBanner;
        private EdgeHintScreen _edgeHint;
        private TickerScreen _ticker;
        private TopBarScreen _top;
        private OperationsScreen _operations;
        private BuildListScreen _buildList;
        private SidePanelScreen _side;
        private FacilityPanelScreen _facility;
        private RouteScreen _route;
        private TitleScreen _title;
        private SaveWindowScreen _save;
        private ClearScreen _clear;
        private PlacingScreen _placing;
        private FacilityMenuScreen _facilityMenu;
        private BlackoutScreen _blackout;
        private TutorialScreen _tutorial;
        private VisualElement _hud;

        public TickerScreen Ticker
        {
            get
            {
                EnsureBuilt();
                return _ticker;
            }
        }

        public ITutorialTargets TutorialTargets
        {
            get
            {
                EnsureBuilt();
                return _targets;
            }
        }

        public WaveBannerScreen WaveBanner => _waveBanner;

        private readonly BoolEdge _facilityOpen = new BoolEdge();
        private readonly BoolEdge _routeOpen = new BoolEdge();
        private readonly BoolEdge _saveOpen = new BoolEdge();
        public event System.Action<Button> ButtonClicked;
        public event System.Action WindowOpened;
        public event System.Action WindowClosed;
        public EdgeHintScreen EdgeHint => _edgeHint;
        private void Build()
        {
            VisualElement root = GetComponent<UIDocument>().rootVisualElement;
            root.pickingMode = PickingMode.Ignore;
            root.RegisterCallback<ClickEvent>(Root_Click, TrickleDown.TrickleDown);
            _hud = Find(root, "hud");
            _waveBanner = Add(new WaveBannerScreen(Find(root, "wave-banner-screen")));
            _edgeHint = Add(new EdgeHintScreen(Find(root, "edge-hint-screen")));
            _ticker = Add(new TickerScreen(Find(root, "ticker-screen")));
            _top = Add(new TopBarScreen(Find(root, "top-bar-screen")));
            _operations = Add(new OperationsScreen(Find(root, "operations-screen")));
            _buildList = Add(new BuildListScreen(Find(root, "build-list-screen"), Find(root, "build-tape-screen")));
            _side = Add(new SidePanelScreen(Find(root, "side-panel-screen")));
            _facility = Add(new FacilityPanelScreen(Find(root, "room-panel-screen")));
            _route = Add(new RouteScreen(Find(root, "route")));
            _title = Add(new TitleScreen(Find(root, "title-screen")));
            _save = Add(new SaveWindowScreen(Find(root, "save-screen")));
            _clear = Add(new ClearScreen(Find(root, "clear-screen")));
            _placing = Add(new PlacingScreen(Find(root, "placing-screen")));
            _facilityMenu = Add(new FacilityMenuScreen(Find(root, "room-menu-screen")));
            _blackout = Add(new BlackoutScreen(Find(root, "blackout-screen")));
            _tutorial = Add(new TutorialScreen(Find(root, "tutorial-screen")));
            _targets = new HUDTutorialTargets(_operations, _buildList, _facility, _route, _top);
            _operations.OpenMenu = OpenMenu;
            _title.OpenSaves = () => _save.Open(SaveScreenMode.Load);
            _clear.AfterNext = () => _save.Open(SaveScreenMode.Ending);
            _controls.AddRange(_operations.Root.Query<Button>().ToList());
            _controls.AddRange(_side.Root.Query<Button>().ToList());
        }

        private void Root_Click(ClickEvent e)
        {
            Button button = (e.target as VisualElement)?.GetFirstOfType<Button>();
            if (button != null && button.IsUsable())
            {
                ButtonClicked?.Invoke(button);
            }
        }

        private void OpenMenu()
        {
            _context.UI.Deselect();
            _facility.Refresh(null);
            _save.Open(SaveScreenMode.Menu);
        }

        public void Bind(UIContext context)
        {
            EnsureBuilt();
            _context = context;
            _targets.Bind(context);
            foreach (UIScreen screen in _screens)
            {
                screen.Bind(context);
            }
        }

        public void Refresh()
        {
            RefreshScreens();
            if (!_title.IsOpen)
            {
                TickWindow(_facilityOpen, IsFacilityOpen);
                TickWindow(_routeOpen, IsRouteOpen);
                TickWindow(_saveOpen, IsSaveScreenOpen);
            }
        }

        private void TickWindow(BoolEdge edge, bool isOpen)
        {
            if (edge.Rose(isOpen))
            {
                WindowOpened?.Invoke();
            }
            else if (edge.Fell)
            {
                WindowClosed?.Invoke();
            }
        }

        private void RefreshScreens()
        {
            _ticker.Refresh();
            _save.Refresh();
            _hud.EnableInClassList("on-title", _title.IsOpen);
            if (_save.IsOpen || _title.IsOpen || _clear.IsOpen)
            {
                _tutorial.Hide();
                _blackout.Refresh();
                return;
            }

            UIState ui = _context.UI;
            _hud.EnableInClassList("cinematic", _context.App.IsCinematic);
            _facility.Refresh(ui.PanelFacility);
            _buildList.Refresh();
            _placing.Refresh();
            _facilityMenu.Refresh(ui.MenuFacility);
            RuinDefinition spokenRuin = SpokenRuin(_context.Session);
            _route.Refresh(ui.IsRouteShown(spokenRuin), spokenRuin);
            _waveBanner.Refresh();
            _edgeHint.Refresh();
            _top.Refresh();
            _side.Refresh();
            _operations.Refresh();
            FillBounds(_menuRects, _operations.MenuButton);
            FillBounds(_timeRects, _operations.TimeButtons);
            FillShownBounds(_controlRects, _controls);
            Rect field = Rect.MinMaxRect(0f, _top.Panel.worldBound.yMax, _operations.Panel.worldBound.xMax, _operations.Panel.worldBound.yMin);
            _tutorial.Refresh(new TutorialFrame(_menuRects, _timeRects, _controlRects, field, _route.CardOf(spokenRuin)));
            _blackout.Refresh();
        }

        private static RuinDefinition SpokenRuin(Session session)
        {
            ITalkSpeaker speaker = session.Speaker;
            return speaker.ShowsTalk ? speaker.Line.About : null;
        }

        public Vector2? PanelSize
        {
            get
            {
                IPanel panel = _hud?.panel;
                return panel != null ? panel.ScreenToPanel(new Vector2(Screen.width, 0f)) : (Vector2?)null;
            }
        }

        public bool IsSaveScreenOpen => _save.IsOpen;
        public bool IsFacilityOpen => _facility.IsOpen;
        public bool IsRouteOpen => _route.IsOpen;
        public bool IsTitleOpen => _title.IsOpen;

        public void OpenTitle()
        {
            EnsureBuilt();
            _title.Open();
        }

        public void OpenSaveScreen(SaveScreenMode mode)
        {
            EnsureBuilt();
            _save.Open(mode);
        }

        public void OpenClearScreen()
        {
            EnsureBuilt();
            _clear.Open();
        }

        public bool IsClearScreenOpen => _clear.IsOpen;

        private void EnsureBuilt()
        {
            if (_screens.Count == 0)
            {
                Build();
            }
        }

        private T Add<T>(T screen) where T : UIScreen
        {
            _screens.Add(screen);
            return screen;
        }

        private static VisualElement Find(VisualElement root, string name)
        {
            return root.Need<VisualElement>(name);
        }

        private static void FillBounds(List<Rect> into, params VisualElement[] elements)
        {
            into.Clear();
            foreach (VisualElement element in elements)
            {
                into.Add(element.worldBound);
            }
        }

        private static void FillShownBounds(List<Rect> into, List<Button> elements)
        {
            into.Clear();
            foreach (Button element in elements)
            {
                if (element.IsShown() && element.worldBound.width > 0f && element.worldBound.height > 0f)
                {
                    into.Add(element.worldBound);
                }
            }
        }
    }
}
