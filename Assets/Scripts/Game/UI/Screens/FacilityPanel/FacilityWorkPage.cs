using Scrapwalker.Core.Outpost;
using Scrapwalker.Core.Outpost.Facilities;
using Scrapwalker.Core.Outpost.Facilities.Commands;
using Scrapwalker.Core.Outpost.Facilities.Recipes;
using Scrapwalker.Core.Sessions.Tutorial;
using Scrapwalker.Game.UI.Screens.Tutorial;
using Scrapwalker.Game.UI.Widgets;
using Scrapwalker.Game.Wording;
using Scrapwalker.Game.Wording.Words;
using UnityEngine;
using UnityEngine.UIElements;

namespace Scrapwalker.Game.UI.Screens.FacilityPanel
{
    public sealed class FacilityWorkPage : FacilityPage, IRecipeShelfView<bool>
    {
        private readonly FacilityStorePanel _store;
        private readonly FacilityRegisterPanel _register;
        private readonly FacilityQueuePanel _queue;
        private readonly Label _position;
        private readonly Label _status;
        private readonly Label _tier;
        private readonly Meter _hp;
        private readonly VisualElement _hpRow;
        private readonly VisualElement _hpBar;
        private readonly VisualElement _fireRow;
        private readonly Label _fire;
        private readonly VisualElement _stationRow;
        private readonly Button _stationPeace;
        private readonly Button _stationCombat;

        private readonly ShownValue<string> _shownFire = new ShownValue<string>();
        private readonly ShownValue<(TaskStatus, int)> _shownStatus = new ShownValue<(TaskStatus, int)>();

        internal FacilityWorkPage(VisualElement root, FacilityTemplates templates) : base(root)
        {
            _position = Q<Label>("position");
            _status = Q<Label>("status");
            _tier = Q<Label>("tier");
            _hp = new Meter(Q<Label>("hp"), Q("hp-fill"), RoomWords.HPValue);
            _hpRow = Q("hp-row");
            _hpBar = Q("hp-bar");
            _fireRow = Q("fire-row");
            _fire = Q<Label>("fire");
            _stationRow = Q("station-row");
            _stationPeace = Q<Button>("station-peace");
            _stationCombat = Q<Button>("station-combat");
            root.WriteLabel("facility-title", RoomWords.SectionFacility);
            root.WriteLabel("hp-key", RoomWords.HP);
            root.WriteLabel("fire-key", RoomWords.FireInterval);
            root.WriteLabel("station-key", RoomWords.Stationing);
            root.WriteLabel("station-peace-label", RoomWords.StationPeace);
            root.WriteLabel("station-combat-label", RoomWords.StationCombat);
            _store = AddPart(new FacilityStorePanel(Q("store-group"), templates));
            _register = AddPart(new FacilityRegisterPanel(Q("register-panel"), templates));
            _queue = AddPart(new FacilityQueuePanel(Q("right"), templates));
        }

        public VisualElement Right => _queue.Root;
        public VisualElement Tasks => _register.Root;

        protected override void OnBind()
        {
            _hp.Reset();
            _shownStatus.Reset();
            ResetPanels();
        }

        public override VisualElement Revealed => Right;

        public override bool Lists(RecipeDefinition recipe)
        {
            return recipe.Result.ShowShelf(this);
        }

        public bool Task() => true;

        public bool Upgrade() => true;

        public bool Research() => false;

        public override VisualElement GuideTo(RecipeDefinition recipe, int times, out TutorialStep step)
        {
            return _register.GuideToRecipe(recipe, times, out step);
        }

        public override VisualElement FrameOf(RecipeDefinition recipe)
        {
            return _register.RowOf(recipe);
        }

        public override void Open(Facility f)
        {
            ResetPanels();
            _hp.Reset();
            _shownStatus.Reset();
            _position.SetText(RoomWords.Position(f.Cell.Row + 1, f.Definition.Width, f.Definition.Height));
            if (f.BuiltQuality.HasValue)
            {
                _tier.SetText(f.BuiltQuality.Value.Tier());
                _tier.SetQuality(f.BuiltQuality.Value);
            }

            _tier.Show(f.BuiltQuality.HasValue);
            _hpRow.Display(!f.Definition.IsIndestructible);
            _hpBar.Display(!f.Definition.IsIndestructible);
            _fireRow.Display(f.FiringRecipe != null);
            _shownFire.Reset();
            _stationRow.Display(f.Definition.HasStation);
            _stationPeace.OnClick(() => Session.Execute(new SetStationing(f.ID, f.Stationing.WithPeace(!f.Stationing.InPeace))));
            _stationCombat.OnClick(() => Session.Execute(new SetStationing(f.ID, f.Stationing.WithCombat(!f.Stationing.InCombat))));
        }

        public override void Close()
        {
            ResetPanels();
        }

        public override void Refresh(Facility f)
        {
            UpdateDetail(f);
            _store.Refresh(f);
            _register.Refresh(f);
            _queue.Refresh(f);
        }

        private void ResetPanels()
        {
            _store.Reset();
            _register.Reset();
            _queue.Reset();
        }

        private void UpdateDetail(Facility f)
        {
            TaskStatus status = f.Status(World);
            int percent = Mathf.RoundToInt(f.Work.WorkProgress * 100f);
            if (_shownStatus.Change((status, percent)))
            {
                bool stopped = status.IsStop();
                _status.text = status.RoomText(percent);
                _status.SetSeverity(status.SeverityOf());
                bool calm = IsCalm(status);
                _status.EnableInClassList("dim", calm);
                _status.EnableInClassList("ok", !stopped && !calm);
                _status.EnableInClassList("bold", stopped);
            }

            _hp.Set(f.HP, f.MaxHP);

            RecipeDefinition fire = f.FiringRecipe;
            if (fire != null)
            {
                string seconds = Texts.Seconds(fire.Seconds);
                if (_shownFire.Change(seconds))
                {
                    _fire.text = RoomWords.FireIntervalValue(seconds);
                }
            }

            if (f.Definition.HasStation)
            {
                _stationPeace.EnableInClassList("btn--selected", f.Stationing.InPeace);
                _stationCombat.EnableInClassList("btn--selected", f.Stationing.InCombat);
            }
        }

        private static bool IsCalm(TaskStatus status)
        {
            switch (status)
            {
                case TaskStatus.NoTasks:
                case TaskStatus.Paused:
                case TaskStatus.AwaitingBot:
                case TaskStatus.Ready:
                    return true;
                default:
                    return false;
            }
        }
    }
}
