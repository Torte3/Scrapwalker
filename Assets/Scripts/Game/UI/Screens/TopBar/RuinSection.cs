using Scrapwalker.Core.Travel;
using Scrapwalker.Core.Travel.Legs;
using Scrapwalker.Game.Definitions;
using Scrapwalker.Game.UI.Widgets;
using Scrapwalker.Game.Wording.Words;
using UnityEngine.UIElements;

namespace Scrapwalker.Game.UI.Screens.TopBar
{
    internal sealed class RuinSection
    {
        private readonly Label _title;
        private readonly Label _name;
        private readonly ShownValue<(JourneyLeg, RuinDefinition)> _shown = new ShownValue<(JourneyLeg, RuinDefinition)>();

        public RuinSection(VisualElement root)
        {
            _title = root.Need<Label>("ruin-title");
            _name = root.Need<Label>("ruin-name");
        }

        public void Reset()
        {
            _shown.Reset();
        }

        public void Refresh(GameData data, Journey journey)
        {
            RuinDefinition ruin = journey.CurrentRuin;
            if (!_shown.Change((journey.Leg, ruin)))
            {
                return;
            }

            if (journey.IsTraveling)
            {
                _title.text = TopBarWords.RuinMoving;
                _name.text = string.Empty;
            }
            else if (ruin != null)
            {
                _title.text = TopBarWords.RuinGrounded;
                _name.text = data.NameOf(ruin);
            }
            else
            {
                _title.text = string.Empty;
                _name.text = string.Empty;
            }
        }
    }
}
