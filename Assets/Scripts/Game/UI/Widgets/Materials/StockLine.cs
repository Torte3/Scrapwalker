using System.Collections.Generic;
using System.Linq;
using Scrapwalker.Core.Items;
using Scrapwalker.Game.Definitions;
using Scrapwalker.Game.Wording;
using Scrapwalker.Game.Wording.Words;
using UnityEngine.UIElements;
using MaterialDefinition = Scrapwalker.Core.Items.MaterialDefinition;

namespace Scrapwalker.Game.UI.Widgets.Materials
{
    public sealed class StockLine
    {
        private readonly Label _name;
        private readonly Label _count;
        private readonly Label _breakdown;
        private readonly int[] _shown = new int[QualityRules.Count];
        private readonly ShownValue<int> _shownTotal = new ShownValue<int>();

        public StockLine(VisualElement element)
        {
            Element = element;
            _name = element.Need<Label>("stock-name");
            _count = element.Need<Label>("stock-count");
            _breakdown = element.Need<Label>("stock-breakdown");
            Reset();
        }

        public VisualElement Element { get; }

        public void SetMaterial(GameData data, MaterialDefinition type)
        {
            _name.SetText(type.Name(data));
        }

        public void Reset()
        {
            _shownTotal.Reset();
            for (int q = 0; q < _shown.Length; q++)
            {
                _shown[q] = -1;
            }
        }

        public void Set(int[] byQuality)
        {
            int total = 0;
            bool changed = false;
            for (int q = 0; q < _shown.Length; q++)
            {
                total += byQuality[q];
                if (_shown[q] != byQuality[q])
                {
                    _shown[q] = byQuality[q];
                    changed = true;
                }
            }

            if (_shownTotal.Change(total))
            {
                _count.text = Texts.Monospace(total);
                _count.EnableInClassList("dim", total == 0);
            }

            if (!changed)
            {
                return;
            }

            IEnumerable<string> parts = QualityRules.All
                .Where(q => q != Quality.Plain && _shown[(int)q] > 0)
                .Reverse()
                .Select(q => StockWords.Quality(q.WithStars(), _shown[(int)q]));
            _breakdown.SetText(string.Join(StockWords.Separator, parts));
        }
    }
}
