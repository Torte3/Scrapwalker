using System;
using System.Collections.Generic;
using Scrapwalker.Core.Items;
using Scrapwalker.Core.Outpost.Facilities;
using Scrapwalker.Game.UI.Widgets.Materials;
using Scrapwalker.Game.UI.Widgets.QualityFilters;
using Scrapwalker.Game.UI.Widgets;
using Scrapwalker.Game.Wording;
using Scrapwalker.Game.Wording.Words;
using UnityEngine.UIElements;
using MaterialDefinition = Scrapwalker.Core.Items.MaterialDefinition;

namespace Scrapwalker.Game.UI.Screens.FacilityPanel
{
    internal sealed class FacilityTaskRow
    {
        private readonly VisualElement _box;
        private readonly Label _title;
        private readonly Label _state;
        private readonly VisualElement _bots;
        private readonly RowList<TemplateContainer> _botLines;
        private readonly ShownValue<string> _stateText = new ShownValue<string>();
        private Action _moveUp;
        private Action _moveDown;
        private Action _moveToFront;

        public FacilityTaskRow(TemplateContainer element, VisualTreeAsset materialLine)
        {
            Element = element;
            _box = element.Need<VisualElement>("order");
            _title = element.Need<Label>("order-title");
            _state = element.Need<Label>("order-state");
            Loop = element.Need<Button>("order-loop");
            Front = element.Need<Button>("order-front");
            Up = element.Need<Button>("order-up");
            Down = element.Need<Button>("order-down");
            Remove = element.Need<Button>("order-remove");
            Materials = new MaterialList(element.Need<VisualElement>("order-mats"), materialLine);
            Filter = new QualityFilterPicker(element.Need<VisualElement>("order-filter"), element.Need<VisualElement>("order-filter-editor"));
            Bar = new JobBar(element.Need<VisualElement>("order-bar"));
            _bots = element.Need<VisualElement>("order-bots");
            VisualTreeAsset bot = _bots.TakeTemplate("bot-template");
            _botLines = new RowList<TemplateContainer>(_bots, bot.Instantiate, line => line);
            element.WriteLabel("order-loop-label", RoomWords.Repeat);
            element.WriteLabel("order-front-label", RoomWords.Up);
            element.WriteLabel("order-up-label", RoomWords.Up);
            element.WriteLabel("order-down-label", RoomWords.Down);
            element.WriteLabel("order-remove-label", RoomWords.Remove);
        }

        public TemplateContainer Element { get; }
        public WorkTask Task { get; set; }
        public Func<MaterialDefinition, int> Delivered { get; set; }
        public Action<QualityFilter> ApplyFilter { get; set; }
        public Button Loop { get; }
        public Button Front { get; }
        public Button Up { get; }
        public Button Down { get; }
        public Button Remove { get; }
        public MaterialList Materials { get; }
        public QualityFilterPicker Filter { get; }
        public JobBar Bar { get; }

        public void SetTitle(string title)
        {
            _title.SetText(title);
        }

        public void SetState(string text, Severity severity)
        {
            if (_stateText.Change(text))
            {
                _state.text = text;
            }

            _state.SetSeverity(severity);
        }

        public void SetStatus(TaskStatus status)
        {
            _stateText.Reset();
            _state.Show(status);
        }

        public void SetWorking(bool isWorking)
        {
            _box.EnableInClassList("room-order--live", isWorking);
        }

        public void SetLoop(bool canRepeat, bool isRepeating)
        {
            Loop.Show(canRepeat);
            Loop.EnableInClassList("btn--selected", isRepeating);
        }

        public void OnMove(Action up, Action down, Action toFront)
        {
            _moveUp = up;
            _moveDown = down;
            _moveToFront = toFront;
        }

        public void SetMovable(bool canMoveUp, bool canMoveDown, bool canMoveToFront)
        {
            Front.SetUsable(canMoveToFront, _moveToFront);
            Up.SetUsable(canMoveUp, _moveUp);
            Down.SetUsable(canMoveDown, _moveDown);
        }

        public void SetBots(IReadOnlyList<string> lines)
        {
            _botLines.SetCount(lines.Count);
            for (int i = 0; i < lines.Count; i++)
            {
                _botLines[i].Q<Label>("bot").SetText(lines[i]);
            }

            _bots.Display(lines.Count > 0);
        }

    }
}
