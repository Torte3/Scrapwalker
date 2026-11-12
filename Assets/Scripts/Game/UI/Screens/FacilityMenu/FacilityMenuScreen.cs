using System;
using Scrapwalker.Core.Outpost;
using Scrapwalker.Core.Outpost.Construction;
using Scrapwalker.Core.Root;
using Scrapwalker.Core.Sessions;
using Scrapwalker.Game.UI.Widgets;
using Scrapwalker.Game.Wording.Words;
using UnityEngine;
using UnityEngine.UIElements;

namespace Scrapwalker.Game.UI.Screens.FacilityMenu
{
    public sealed class FacilityMenuScreen : UIScreen
    {
        private const float Lift = 14f;

        private readonly VisualElement _menu;
        private readonly Button[] _choices;
        private readonly Label[] _labels;
        private readonly Action[] _actions;

        private readonly ShownValue<(World, int, Facility)> _builtFor = new ShownValue<(World, int, Facility)>();
        private int _used;
        private Vector2 _placed = new Vector2(float.NaN, float.NaN);

        public FacilityMenuScreen(VisualElement root) : base(root)
        {
            _menu = Q("room-menu");
            _choices = new[] { Q<Button>("choice-0"), Q<Button>("choice-1") };
            _labels = new Label[_choices.Length];
            _actions = new Action[_choices.Length];
            for (int i = 0; i < _choices.Length; i++)
            {
                int slot = i;
                _labels[i] = Q<Label>(_choices[i], "choice-label");
                // 先に選択を外す。逆だと Deselect が移設先を選ぶ操作を取り消す
                _choices[i].clicked += () =>
                {
                    Action action = _actions[slot];
                    UIState.Deselect();
                    action?.Invoke();
                };
            }
        }

        protected override void OnBind()
        {
            _builtFor.Reset();
            _menu.Display(false);
        }

        public void Refresh(Facility facility)
        {
            if (facility == null)
            {
                _builtFor.Reset();
                _menu.Display(false);
                return;
            }

            if (_builtFor.Change((World, World.Revision, facility)))
            {
                Fill(facility);
                _menu.Display(_used > 0);
                if (_used == 0)
                {
                    _builtFor.Reset();
                    UIState.Deselect();
                    return;
                }
            }

            Vector2 screen = UIState.IsSelectedAtRelocateTarget
                ? App.GridOnScreen.TopCenterOf(facility.RelocateRect.Value)
                : App.GridOnScreen.TopCenterOf(facility.Rect);
            Place(screen);
        }

        private void Place(Vector2 screen)
        {
            if (_menu.panel == null || _menu.parent == null)
            {
                return;
            }

            Vector2 onPanel = _menu.panel.ScreenToPanel(screen);
            Vector2 local = _menu.parent.WorldToLocal(onPanel) - new Vector2(0f, Lift);
            if (local == _placed)
            {
                return;
            }

            _placed = local;
            _menu.style.left = local.x;
            _menu.style.top = local.y;
        }

        private void Fill(Facility f)
        {
            _used = 0;
            AddChoices(f);
            for (int i = 0; i < _choices.Length; i++)
            {
                _choices[i].Display(i < _used);
                if (i >= _used)
                {
                    _actions[i] = null;
                }
            }
        }

        private void AddChoices(Facility f)
        {
            bool relocate = f.Condition.Refusal(FacilityOrder.Relocate) == CommandError.None;
            if (Session.Preview(new CancelPlacement(f.ID)).Own.IsOK)
            {
                if (relocate)
                {
                    Add(RoomMenuWords.ChangeSite, "cyan", () => UIState.Build.BeginRelocate(f));
                }

                Add(RoomMenuWords.CancelBuild, "warn", () => Session.Execute(new CancelPlacement(f.ID)));
                return;
            }

            if (Session.Preview(new CancelDismantle(f.ID)).Own.IsOK)
            {
                if (relocate)
                {
                    Add(RoomMenuWords.ChangeTarget, "cyan", () => UIState.Build.BeginRelocate(f));
                }

                Add(f.Condition.RelocateTarget.HasValue ? RoomMenuWords.CancelRelocate : RoomMenuWords.CancelDismantle, "warn", () => Session.Execute(new CancelDismantle(f.ID)));
                return;
            }

            if (relocate && UIState.IsBuildMode)
            {
                Add(RoomMenuWords.Relocate, "cyan", () => UIState.Build.BeginRelocate(f));
            }

            AddDismantle(f);
        }

        private void AddDismantle(Facility f)
        {
            if (UIState.IsBuildMode && Session.Preview(new DismantleFacility(f.ID)).Own.IsOK)
            {
                Add(RoomMenuWords.Dismantle, "warn", () => Session.Execute(new DismantleFacility(f.ID)));
            }
        }

        private void Add(string label, string color, Action action)
        {
            int slot = _used++;
            _labels[slot].text = label;
            _choices[slot].SetVariant("room-menu__choice--", color);
            _actions[slot] = action;
        }
    }
}
