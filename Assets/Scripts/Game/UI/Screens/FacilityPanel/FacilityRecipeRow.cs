using System;
using System.Collections.Generic;
using Scrapwalker.Core.Outpost.Facilities;
using Scrapwalker.Game.Definitions;
using Scrapwalker.Game.UI.Widgets.Materials;
using Scrapwalker.Game.UI.Widgets.QualityFilters;
using Scrapwalker.Game.UI.Widgets;
using Scrapwalker.Game.Wording;
using Scrapwalker.Game.Wording.Words;
using UnityEngine.UIElements;
using MaterialDefinition = Scrapwalker.Core.Items.MaterialDefinition;

namespace Scrapwalker.Game.UI.Screens.FacilityPanel
{
    internal sealed class FacilityRecipeRow
    {
        private readonly VisualElement _box;
        private readonly VisualElement _mark;
        private readonly Label _title;
        private readonly Label _time;
        private readonly VisualElement _body;
        private readonly VisualElement _dash;
        private readonly Label _reject;
        private readonly VisualElement _yield;
        private readonly Label _yieldName;
        private readonly Label _yieldCount;
        private readonly Label _yieldHeld;
        private readonly VisualElement _effects;
        private readonly RowList<TemplateContainer> _effectLines;
        private readonly Label _note;
        private readonly VisualElement _times;
        private readonly VisualElement _acts;
        private readonly Label _count;
        private readonly Label _onceLabel;
        private readonly Label _repeatLabel;
        private readonly ShownValue<MaterialDefinition> _shownYield = new ShownValue<MaterialDefinition>();
        private readonly ShownValue<int> _shownYieldCount = new ShownValue<int>();
        private readonly ShownValue<int> _shownYieldHeld = new ShownValue<int>();
        private readonly ShownValue<int> _shownCount = new ShownValue<int>();
        private readonly ShownValue<(int, bool)> _shownLabels = new ShownValue<(int, bool)>();
        private bool _once;
        private Action _registerOnce;
        private Action _registerRepeat;

        public FacilityRecipeRow(TemplateContainer element, VisualTreeAsset materialLine)
        {
            Element = element;
            _box = element.Need<VisualElement>("recipe");
            Head = element.Need<VisualElement>("recipe-head");
            _mark = element.Need<VisualElement>("recipe-mark");
            _title = element.Need<Label>("recipe-title");
            _time = element.Need<Label>("recipe-time");
            _body = element.Need<VisualElement>("recipe-body");
            _dash = element.Need<VisualElement>("recipe-dash");
            Materials = new MaterialList(element.Need<VisualElement>("recipe-mats"), materialLine);
            _reject = element.Need<Label>("recipe-reject");
            _yield = element.Need<VisualElement>("recipe-yield");
            _yieldName = element.Need<Label>("yield-name");
            _yieldCount = element.Need<Label>("yield-count");
            _yieldHeld = element.Need<Label>("yield-held");
            _effects = element.Need<VisualElement>("recipe-effects");
            VisualTreeAsset effect = _effects.TakeTemplate("effect-template");
            _effectLines = new RowList<TemplateContainer>(_effects, () => effect.Instantiate().Ignored(), line => line);
            _note = element.Need<Label>("recipe-note");
            _times = element.Need<VisualElement>("recipe-times");
            _acts = element.Need<VisualElement>("recipe-acts");
            Fewer = element.Need<Button>("recipe-fewer");
            _count = element.Need<Label>("recipe-count");
            More = element.Need<Button>("recipe-more");
            Filter = new QualityFilterPicker(element.Need<VisualElement>("recipe-filter"), element.Need<VisualElement>("recipe-filter-editor"));
            Once = element.Need<Button>("recipe-once");
            Repeat = element.Need<Button>("recipe-repeat");
            _onceLabel = element.Need<Label>("recipe-once-label");
            _repeatLabel = element.Need<Label>("recipe-repeat-label");
            element.WriteLabel("recipe-times-key", RoomWords.Times);
            element.WriteLabel("recipe-fewer-label", RoomWords.Fewer);
            element.WriteLabel("recipe-more-label", RoomWords.More);
            Head.RegisterCallback<ClickEvent>(_ => Toggled?.Invoke());
            _body.RegisterCallback<GeometryChangedEvent>(_ =>
            {
                if (IsOpen)
                {
                    Element.GetFirstAncestorOfType<ScrollView>()?.ScrollTo(_box);
                }
            });
        }

        public TemplateContainer Element { get; }
        public VisualElement Box => _box;
        public VisualElement Head { get; }
        public RecipeDefinition Recipe { get; set; }
        public bool IsOpen { get; private set; }

        public Action Toggled { get; set; }

        public MaterialList Materials { get; }
        public Button Fewer { get; }
        public Button More { get; }
        public QualityFilterPicker Filter { get; }
        public Button Once { get; }
        public Button Repeat { get; }

        public void SetHead(string title, string time, bool open)
        {
            IsOpen = open;
            _title.SetText(title);
            _time.SetText(time);
            _box.EnableInClassList("room-recipe--open", open);
            _mark.EnableInClassList("icon--tri-right", !open);
            _mark.EnableInClassList("icon--tri-down", open);
            _body.Display(open);
            _dash.Show(!open);
            _note.Display(!open && _note.text.Length > 0);
        }

        public void SetBlocked(string blocked)
        {
            _note.SetText(blocked);
            _box.EnableInClassList("room-recipe--blocked", blocked.Length > 0);
            _note.Display(!IsOpen && blocked.Length > 0);
        }

        public void SetYield(GameData data, MaterialDefinition type, int count, int held)
        {
            _yield.Display(type != null);
            if (type == null)
            {
                return;
            }

            if (_shownYield.Change(type))
            {
                _yieldHeld.RemoveFromClassList("room-yield__held--over");
                _yieldName.text = type.Name(data);
            }

            if (_shownYieldCount.Change(count))
            {
                _yieldCount.text = Texts.Times(count);
            }

            if (_shownYieldHeld.Change(held))
            {
                _yieldHeld.text = RoomWords.YieldHeld(held);
            }
        }

        public void SetEnergyYield(GameData data, float amount, float stored, float capacity)
        {
            _yield.Display(true);
            if (_shownYield.Change(null))
            {
                _yieldName.text = MaterialText.Energy(data);
            }

            _shownYieldCount.Reset();
            _shownYieldHeld.Reset();

            _yieldCount.SetText(Texts.Times(EnergyText.Need(amount)));
            _yieldHeld.SetText(RoomWords.YieldStored(EnergyText.Amount(stored), EnergyText.Amount(capacity)));
            _yieldHeld.EnableInClassList("room-yield__held--over", stored + amount > capacity);
        }

        public void SetEffects(IReadOnlyList<FacilityEffect> lines)
        {
            _effects.Display(lines.Count > 0);
            _effectLines.SetCount(lines.Count);
            for (int i = 0; i < lines.Count; i++)
            {
                TemplateContainer line = _effectLines[i];
                line.Q<Label>("effect").SetText(lines[i].Text);
                Label note = line.Q<Label>("effect-note");
                note.SetText(lines[i].Note);
                note.Display(lines[i].Note.Length > 0);
            }
        }

        public void OnRegister(Action once, Action repeat)
        {
            _registerOnce = once;
            _registerRepeat = repeat;
        }

        public void SetReject(string reject)
        {
            _reject.SetText(reject);
            _reject.Display(reject.Length > 0);
            bool canRegister = reject.Length == 0;
            Once.SetUsable(canRegister, _registerOnce);
            Repeat.SetUsable(canRegister, _registerRepeat);
        }

        public void SetTimes(int times, bool once)
        {
            _once = once;
            _times.EnableInClassList("room-recipe__times--once", once);
            Repeat.Display(!once);
            _times.Display(!_once || Filter.IsShown);
            _acts.EnableInClassList("room-recipe__acts--gap", !Filter.IsOpen);
            if (_shownCount.Change(times))
            {
                _count.text = times.ToString();
            }

            if (_shownLabels.Change((times, once)))
            {
                _onceLabel.text = once ? RoomWords.Run : RoomWords.RunTimes(times);
                _repeatLabel.text = RoomWords.RepeatTimes(times);
            }
        }

    }
}
