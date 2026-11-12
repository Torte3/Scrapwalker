using System;
using System.Collections.Generic;
using Scrapwalker.Core.Items;
using Scrapwalker.Game.Definitions;
using UnityEngine.UIElements;
using MaterialDefinition = Scrapwalker.Core.Items.MaterialDefinition;

namespace Scrapwalker.Game.UI.Widgets.Materials
{
    public sealed class StockList
    {
        private readonly RowList<StockLine> _lines;
        private readonly int[] _byQuality = new int[QualityRules.Count];
        private readonly bool _onlyHeld;
        private IReadOnlyList<MaterialDefinition> _materials = Array.Empty<MaterialDefinition>();

        public StockList(VisualElement parent, Func<VisualElement> make, bool onlyHeld = false)
        {
            _onlyHeld = onlyHeld;
            _lines = new RowList<StockLine>(parent, () => new StockLine(make()), line => line.Element);
        }

        public void Rebuild(GameData data, IReadOnlyList<MaterialDefinition> materials)
        {
            _materials = materials;
            _lines.SetCount(materials.Count);
            for (int t = 0; t < materials.Count; t++)
            {
                _lines[t].SetMaterial(data, materials[t]);
                _lines[t].Reset();
            }
        }

        public int Refresh(Func<MaterialDefinition, Quality, int> countOf)
        {
            int held = 0;
            for (int t = 0; t < _materials.Count; t++)
            {
                int total = 0;
                foreach (Quality q in QualityRules.All)
                {
                    _byQuality[(int)q] = countOf(_materials[t], q);
                    total += _byQuality[(int)q];
                }

                _lines[t].Set(_byQuality);
                if (total > 0)
                {
                    held++;
                }

                if (_onlyHeld)
                {
                    _lines[t].Element.Display(total > 0);
                }
            }

            return held;
        }
    }
}
