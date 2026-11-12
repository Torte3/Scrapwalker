using System;
using System.Collections.Generic;
using Scrapwalker.Core.Items;
using Scrapwalker.Game.Wording;
using Scrapwalker.Game.Wording.Words;
using UnityEngine.UIElements;

namespace Scrapwalker.Game.UI.Widgets.QualityFilters
{
    public sealed class QualityFilterEditor
    {
        private readonly Label _summary;
        private readonly QualityRangeSlider _range;
        private readonly Label[] _ticks;
        private readonly ButtonChoice<bool> _order;
        private readonly Action<Quality, Quality> _onRange;
        private Action<QualityFilter> _apply;
        private QualityFilter _filter;
        private readonly ShownValue<(QualityFilter, ShownQualities)> _shape = new ShownValue<(QualityFilter, ShownQualities)>();
        private bool _ticksNamed;

        public QualityFilterEditor(VisualElement element)
        {
            Root = element.name == "quality-filter" ? element : element.Need<VisualElement>("quality-filter");
            _summary = Root.Need<Label>("quality-filter-summary");
            _range = new QualityRangeSlider(Root.Need<VisualElement>("quality-filter-range"));
            List<Label> ticks = Root.Query<Label>(className: "quality-filter__tick").ToList();
            _ticks = ticks.ToArray();
            _order = new ButtonChoice<bool>("btn--selected", isHighFirst => Apply(_filter.WithHighFirst(isHighFirst)));
            _order.Add(false, Root.Need<Button>("quality-filter-low-first"));
            _order.Add(true, Root.Need<Button>("quality-filter-high-first"));
            Root.WriteLabel("quality-filter-range-key", FilterWords.RangeTitle);
            Root.WriteLabel("quality-filter-order-key", FilterWords.OrderTitle);
            Root.WriteLabel("quality-filter-low-first-label", FilterWords.LowFirst);
            Root.WriteLabel("quality-filter-high-first-label", FilterWords.HighFirst);
            _onRange = (low, high) => Apply(_filter.WithRange(low, high));
        }

        public VisualElement Root { get; }

        public void Set(QualityFilter filter, ShownQualities shown, Action<QualityFilter> apply)
        {
            _apply = apply;
            if (!_ticksNamed)
            {
                _ticksNamed = true;
                for (int i = 0; i < _ticks.Length; i++)
                {
                    _ticks[i].text = ((Quality)i).Tier();
                }
            }

            _range.Set(filter.Min, filter.Max, shown, _onRange);
            if (!_shape.Change((filter, shown)))
            {
                return;
            }

            _filter = filter;
            for (int i = 0; i < _ticks.Length; i++)
            {
                _ticks[i].Display(shown.Contains((Quality)i));
            }

            _summary.text = Summary(filter, shown);
            _order.Select(filter.IsHighFirst);
        }

        private static string Summary(QualityFilter filter, ShownQualities shown)
        {
            if (!shown.TryGetEnds(filter.Min, filter.Max, out Quality low, out Quality high))
            {
                return string.Empty;
            }

            if (low == high)
            {
                return low.Tier();
            }

            return FilterWords.Range(low.Tier(), high.Tier());
        }

        private void Apply(QualityFilter chosen)
        {
            _apply?.Invoke(chosen);
        }
    }
}
