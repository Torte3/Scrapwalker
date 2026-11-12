using Scrapwalker.Core.Root;
using Scrapwalker.Game.Definitions;
using Scrapwalker.Game.UI.Widgets.Materials;
using Scrapwalker.Game.UI.Widgets;
using Scrapwalker.Game.Wording;
using Scrapwalker.Game.Wording.Words;
using UnityEngine.UIElements;

namespace Scrapwalker.Game.UI.Screens.TopBar
{
    internal sealed class PowerSection
    {
        private readonly Label _value;
        private readonly Meter _power;
        private readonly VisualElement _icon;

        public PowerSection(VisualElement root)
        {
            _value = root.Need<Label>("power-value");
            _power = new Meter(_value, root.Need<VisualElement>("power-fill"), (now, max) => Texts.Monospace(TopBarWords.PowerValue(now, max)));
            _icon = root.Need<VisualElement>("power-icon");
        }

        public void Reset(GameData data)
        {
            _icon.SetEnergy(data.Art.Screen);
            _power.Reset();
        }

        public void Refresh(World world)
        {
            int now = EnergyText.Amount(world.Energy.Stored);
            if (_power.Set(now, EnergyText.Amount(world.Energy.Capacity), world.Energy.Fill))
            {
                _value.EnableInClassList("top-bar__power-value--empty", now <= 0);
            }
        }
    }
}
