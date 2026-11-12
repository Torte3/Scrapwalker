using Scrapwalker.Game.Definitions;
using Scrapwalker.Game.Wording;
using UnityEngine.UIElements;
using MaterialDefinition = Scrapwalker.Core.Items.MaterialDefinition;

namespace Scrapwalker.Game.UI.Widgets.Materials
{
    public sealed class MaterialLine
    {
        private readonly VisualElement _line;
        private readonly Label _name;
        private readonly Label _need;
        private readonly Label _have;
        private readonly ShownValue<int> _shownNeed = new ShownValue<int>();
        private readonly ShownValue<int> _shownHave = new ShownValue<int>();

        public MaterialLine(VisualElement element)
        {
            Element = element;
            _line = element.Need<VisualElement>("mat");
            _name = element.Need<Label>("mat-name");
            _need = element.Need<Label>("mat-need");
            _have = element.Need<Label>("mat-have");
        }

        public VisualElement Element { get; }

        public void SetMaterial(GameData data, MaterialDefinition type)
        {
            _name.SetText(type.Name(data));
        }

        public void SetCounts(int need, int have, int delivered, bool isShort)
        {
            if (_shownNeed.Change(need))
            {
                _need.text = need.ToString();
            }

            if (_shownHave.Change(have))
            {
                _have.text = MaterialText.Have(have);
            }

            _line.RemoveFromClassList("mat--plain");
            bool done = delivered >= need;
            _line.EnableInClassList("mat--delivered", done);
            _line.EnableInClassList("warn", !done && isShort);
        }

        public void SetEnergy(GameData data, float need, float stored, bool taken)
        {
            _name.SetText(MaterialText.Energy(data));
            _shownNeed.Reset();
            _shownHave.Reset();
            _need.text = EnergyText.Need(need);
            _have.text = MaterialText.Have(EnergyText.Amount(stored));
            _line.RemoveFromClassList("mat--plain");
            _line.EnableInClassList("mat--delivered", taken);
            _line.EnableInClassList("warn", !taken && stored < need);
        }

        public void SetPlain(string name, string value)
        {
            _name.SetText(name);
            _have.SetText(value);
            _shownNeed.Reset();
            _shownHave.Reset();
            _line.AddToClassList("mat--plain");
            _line.RemoveFromClassList("mat--delivered");
            _line.RemoveFromClassList("warn");
        }
    }
}
