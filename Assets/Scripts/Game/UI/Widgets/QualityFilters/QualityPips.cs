using Scrapwalker.Core.Items;
using UnityEngine.UIElements;

namespace Scrapwalker.Game.UI.Widgets.QualityFilters
{
    public sealed class QualityPips
    {
        private readonly VisualElement[] _cells;
        private readonly VisualElement[] _lowMarks;
        private readonly VisualElement[] _highMarks;
        private readonly string _onClass;

        public QualityPips(VisualElement[] cells, string lowMarkClass, string highMarkClass, string onClass)
        {
            _cells = cells;
            _onClass = onClass;
            _lowMarks = new VisualElement[cells.Length];
            _highMarks = new VisualElement[cells.Length];
            for (int i = 0; i < cells.Length; i++)
            {
                _lowMarks[i] = cells[i].NeedClass(lowMarkClass);
                _highMarks[i] = cells[i].NeedClass(highMarkClass);
            }
        }

        public void Paint(ShownQualities shown, Quality min, Quality max, bool showsLow, bool showsHigh)
        {
            bool hasEnds = shown.TryGetEnds(min, max, out Quality low, out Quality high);
            for (int i = 0; i < _cells.Length; i++)
            {
                var q = (Quality)i;
                _cells[i].Display(shown.Contains(q));
                _cells[i].EnableInClassList(_onClass, hasEnds && q >= low && q <= high);
                _lowMarks[i].Display(showsLow && hasEnds && q == low);
                _highMarks[i].Display(showsHigh && hasEnds && q == high);
            }
        }
    }
}
