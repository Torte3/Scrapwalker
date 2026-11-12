using System;
using Scrapwalker.Core.Items;
using UnityEngine.UIElements;

namespace Scrapwalker.Game.UI.Widgets.QualityFilters
{
    public sealed class QualityFilterIcon
    {
        private readonly QualityPips _pips;
        private readonly ShownValue<(QualityFilter, bool, ShownQualities)> _shape = new ShownValue<(QualityFilter, bool, ShownQualities)>();

        public QualityFilterIcon(VisualElement element)
        {
            Button = element as Button ?? element.Need<Button>("quality-filter-icon");
            VisualElement[] pips = Button.Query(className: "quality-filter-icon__pip").ToList().ToArray();
            _pips = new QualityPips(pips, "quality-filter-icon__mark--low", "quality-filter-icon__mark--high", "quality-filter-icon__pip--on");
        }

        public Button Button { get; }

        public void OnClick(Action action)
        {
            Button.OnClick(action);
        }

        public void Set(QualityFilter filter, bool open, ShownQualities shown)
        {
            if (!_shape.Change((filter, open, shown)))
            {
                return;
            }

            _pips.Paint(shown, filter.Min, filter.Max, !filter.IsHighFirst, filter.IsHighFirst);
            Button.EnableInClassList("quality-filter-icon--open", open);
        }
    }
}
