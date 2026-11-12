using System.Collections.Generic;
using System.Linq;
using Scrapwalker.Core.Travel;
using Scrapwalker.Game.Definitions;
using Scrapwalker.Game.UI.Widgets;
using Scrapwalker.Game.Wording;
using Scrapwalker.Game.Wording.Words;
using UnityEngine.UIElements;
using MaterialDefinition = Scrapwalker.Core.Items.MaterialDefinition;

namespace Scrapwalker.Game.UI.Screens.Route
{
    public sealed class DestinationCard
    {
        private readonly VisualElement _card;
        private readonly Label _name;
        private readonly VisualElement _cleared;
        private readonly Label _mainItems;
        private readonly Label _landLabel;

        public DestinationCard(TemplateContainer instance)
        {
            Root = instance;
            _card = instance.Need<VisualElement>("card");
            _name = instance.Need<Label>("name");
            _cleared = instance.Need<VisualElement>("cleared");
            _mainItems = instance.Need<Label>("main-items");
            Land = instance.Need<Button>("land");
            Repeat = instance.Need<Button>("repeat");
            _landLabel = instance.Need<Label>("land-label");
            instance.WriteLabel("cleared", RouteWords.Cleared);
            instance.WriteLabel("repeat-label", RouteWords.Repeat);
        }

        public TemplateContainer Root { get; }
        public Button Land { get; }
        public Button Repeat { get; }
        public RuinDefinition Ruin { get; private set; }

        public float NameWidth => _name.layout.width;

        public void SetNameWidth(StyleLength width)
        {
            _name.style.width = width;
        }

        public void Fill(GameData data, RuinDefinition ruin, bool isCleared, DestinationStatus status)
        {
            Ruin = ruin;
            _name.text = data.NameOf(ruin);
            _cleared.Show(isCleared);
            List<MaterialDefinition> items = ruin.MainItems.Select(main => main.Definition).Where(type => type != null).ToList();
            _mainItems.Display(items.Count > 0);
            _mainItems.text = items.Count > 0 ? RouteWords.MainItems(string.Join(RouteWords.MainItemsSeparator, items.Select(type => type.Name(data)))) : string.Empty;
            _card.EnableInClassList("route-card--unreachable", !status.IsReachable);
            Land.EnableInClassList("route-card__land--usable", status.CanChoose);
            Repeat.Show(status.CanRepeat);
            _landLabel.text = LandText(status);
        }

        private static string LandText(DestinationStatus status)
        {
            if (status.IsHeading)
            {
                return RouteWords.Heading;
            }

            if (!status.IsReachable)
            {
                return RouteWords.Unreachable;
            }

            if (!status.IsOffRuin)
            {
                return RouteWords.NotMoving;
            }

            return status.IsDecided ? RouteWords.Decided : RouteWords.Land;
        }
    }
}
