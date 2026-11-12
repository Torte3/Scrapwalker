using System;
using System.Linq;
using Scrapwalker.Core.Root;
using Scrapwalker.Core.Sessions;
using Scrapwalker.Core.Travel;
using Scrapwalker.Core.Travel.Legs;
using Scrapwalker.Game.UI.Widgets;
using Scrapwalker.Game.Wording.Words;
using UnityEngine.UIElements;

namespace Scrapwalker.Game.UI.Screens.Operations
{
    public sealed class OperationsScreen : UIScreen
    {
        private const int NormalSpeed = 1;
        private const int FastSpeed = 2;
        private const int FasterSpeed = 3;
        private const float DoubleClickSeconds = 0.3f;

        private readonly VisualElement _center;
        private readonly VisualElement _right;
        private readonly ButtonChoice<int> _speeds;
        private readonly VisualElement _fastIcon;
        private readonly VisualElement _actIcon;
        private readonly Label _actLabel;
        private readonly ShownValue<bool> _buildModeShown = new ShownValue<bool>();
        private readonly ShownValue<(bool, JourneyLeg, bool, bool)> _actShown = new ShownValue<(bool, JourneyLeg, bool, bool)>();
        private readonly ShownValue<int> _speedShown = new ShownValue<int>();
        private Action _onAct;
        private float _fastPressedAt = float.NegativeInfinity;

        public OperationsScreen(VisualElement root) : base(root)
        {
            BuildButton = Q<Button>("build");
            CloseBuildButton = Q<Button>("close-build");
            _center = Q("ops-center");
            _right = Q("ops-right");
            Button fast = Q<Button>("fast");
            _speeds = new ButtonChoice<int>("btn--selected", ChooseSpeed);
            _speeds.Add(0, Q<Button>("pause"));
            _speeds.Add(NormalSpeed, Q<Button>("normal"));
            _speeds.Add(FastSpeed, fast);
            TimeButtons = _speeds.Buttons.ToArray();
            _fastIcon = Q<VisualElement>(fast, "fast-icon");
            ActionButton = Q<Button>("act");
            _actIcon = Q("act-icon");
            _actLabel = Q<Label>("act-label");
            MenuButton = Q<Button>("menu");
            Panel = Q("ops-panel");
            root.WriteLabel("build-label", OperationsWords.Build);
            root.WriteLabel("close-build-label", OperationsWords.Close);
            root.WriteLabel("stock-label", OperationsWords.Stock);
            root.WriteLabel("bots-label", OperationsWords.Bots);
            root.WriteLabel("settings-label", OperationsWords.Settings);
            root.WriteLabel("menu-label", OperationsWords.Menu);

            BuildButton.clicked += () => UIState.SetBuildMode(!UIState.IsBuildMode);
            CloseBuildButton.clicked += () => UIState.SetBuildMode(false);

            Q<Button>("stock").clicked += () => UIState.OpenSide(SidePage.Stock);
            Q<Button>("bots").clicked += () => UIState.OpenSide(SidePage.Bots);
            Q<Button>("settings").clicked += () => UIState.OpenSide(SidePage.Settings);
            MenuButton.clicked += () => OpenMenu?.Invoke();
            ActionButton.clicked += () => _onAct?.Invoke();
        }

        public Button BuildButton { get; }
        public Button CloseBuildButton { get; }
        public Button ActionButton { get; }
        public Button MenuButton { get; }
        public Button[] TimeButtons { get; }
        public VisualElement Panel { get; }

        public Action OpenMenu { get; set; }

        protected override void OnBind()
        {
            _buildModeShown.Reset();
            _speedShown.Reset();
            _actShown.Reset();
        }

        public void Refresh()
        {
            bool isBuildMode = UIState.IsBuildMode;
            if (_buildModeShown.Change(isBuildMode))
            {
                BuildButton.Display(!isBuildMode);
                CloseBuildButton.Display(isBuildMode);
                _center.Show(!isBuildMode);
                _right.Show(!isBuildMode);
            }

            Journey journey = World.Journey;
            if (_actShown.Change((World.IsDefeated, journey.Leg, journey.IsAtRuin, journey.Destination != null)))
            {
                RefreshAction(journey, World.IsDefeated);
            }

            int speed = Session.Time.Speed;
            if (_speedShown.Change(speed))
            {
                _speeds.Select(Math.Min(speed, FastSpeed));

                _fastIcon.EnableInClassList("icon--faster", speed == FasterSpeed);
            }
        }

        private void ChooseSpeed(int speed)
        {
            if (speed == FastSpeed)
            {
                PressFast();
            }
            else
            {
                Session.SetSpeed(speed);
            }
        }

        private void PressFast()
        {
            float now = UnityEngine.Time.realtimeSinceStartup;
            bool isDoubleClick = now - _fastPressedAt <= DoubleClickSeconds;
            _fastPressedAt = now;
            if (isDoubleClick)
            {
                Session.SetSpeed(FasterSpeed);
                return;
            }

            Session.SetSpeed(Session.Time.Speed == FasterSpeed ? NormalSpeed : FastSpeed);
        }

        private void RefreshAction(Journey journey, bool isDefeated)
        {
            ActionButton.Show(!isDefeated);
            if (isDefeated)
            {
                _onAct = null;
                return;
            }

            if (journey.IsTraveling && journey.Destination != null)
            {
                Paint(OperationsWords.CancelRoute, "close", "plain");
                _onAct = () => Session.Execute(new CancelDestination());
                return;
            }

            if (!journey.IsAtRuin)
            {
                Paint(OperationsWords.Route, "route", "cyan");
                _onAct = () =>
                {
                    UIState.Deselect();
                    UIState.ToggleRoute();
                };
                return;
            }

            Paint(OperationsWords.Leave, "depart", "warn");
            _onAct = () => Session.Execute(new Depart());
        }

        private void Paint(string label, string icon, string color)
        {
            _actLabel.text = label;
            _actIcon.SetVariant("icon--", icon);
            ActionButton.SetVariant("ops-primary--", color);
        }
    }
}
