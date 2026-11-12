using System;
using System.Collections.Generic;
using Scrapwalker.Core.Items;
using Scrapwalker.Core.Outpost;
using Scrapwalker.Game.Definitions;
using UnityEngine.UIElements;
using MaterialDefinition = Scrapwalker.Core.Items.MaterialDefinition;

namespace Scrapwalker.Game.UI.Widgets.Materials
{
    public sealed class MaterialList
    {
        private readonly RowList<MaterialLine> _rows;
        private IReadOnlyList<MaterialAmount> _amounts = Array.Empty<MaterialAmount>();
        private int _materials;
        private bool _hasEnergy;

        public MaterialList(VisualElement parent, VisualTreeAsset template, string rowClass = null)
        {
            _rows = new RowList<MaterialLine>(parent, () => new MaterialLine(Row(template, rowClass)), line => line.Element);
        }

        public void Show(GameData data, IReadOnlyList<MaterialAmount> amounts)
        {
            Show(data, amounts, amounts.Count, false);
        }

        public void Show(GameData data, IReadOnlyList<MaterialAmount> amounts, int count)
        {
            Show(data, amounts, count, false);
        }

        public void ShowWithEnergy(GameData data, IReadOnlyList<MaterialAmount> amounts, float energy)
        {
            Show(data, amounts, amounts.Count, energy > 0f);
        }

        public void SetCounts(Ledger ledger, QualityFilter filter, int times = 1, Func<MaterialDefinition, int> delivered = null)
        {
            List<MaterialAmount> shortage = ledger.Shortage(_amounts, filter, delivered, times);
            for (int i = 0; i < _materials; i++)
            {
                MaterialAmount a = _amounts[i];
                _rows[i].SetCounts(a.Count * times, ledger.HeldMaterial(a.Type, filter), delivered != null ? delivered(a.Type) : 0, shortage.Exists(s => s.Type == a.Type));
            }
        }

        public void SetEnergy(GameData data, float need, float stored, bool taken)
        {
            if (_hasEnergy)
            {
                _rows[_materials].SetEnergy(data, need, stored, taken);
            }
        }

        public void AddPlain(string name, string value)
        {
            _rows.Next().SetPlain(name, value);
            _rows.End();
        }

        private void Show(GameData data, IReadOnlyList<MaterialAmount> amounts, int count, bool withEnergy)
        {
            _amounts = amounts;
            _materials = count;
            _hasEnergy = withEnergy;
            _rows.SetCount(count + (withEnergy ? 1 : 0));
            for (int i = 0; i < count; i++)
            {
                _rows[i].SetMaterial(data, amounts[i].Type);
            }
        }

        private static VisualElement Row(VisualTreeAsset template, string rowClass)
        {
            TemplateContainer row = template.Instantiate().Ignored();
            if (rowClass != null)
            {
                row.AddToClassList(rowClass);
            }

            return row;
        }
    }
}
