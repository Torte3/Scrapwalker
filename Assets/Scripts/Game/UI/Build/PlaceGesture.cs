using Scrapwalker.Core.Items;
using Scrapwalker.Core.Outpost;
using Scrapwalker.Core.Outpost.Construction;
using Scrapwalker.Core.Outpost.Grid;
using Scrapwalker.Core.Root;
using Scrapwalker.Game.Wording.Words;

namespace Scrapwalker.Game.UI.Build
{
    public sealed class PlaceGesture : BuildGesture
    {
        private readonly FacilityDefinition _definition;

        public PlaceGesture(FacilityDefinition definition)
        {
            _definition = definition;
        }

        public override FacilityDefinition Definition => _definition;
        public override FacilityDefinition PlacingDefinition => _definition;
        internal override string ClickText => WorldWords.ClickPlace;

        internal override Command CommandAt(Cell at, QualityFilter filter)
        {
            return new PlaceFacility(_definition, at, filter);
        }

        internal override bool ContinuesAfterRun(bool keepPlacing)
        {
            return keepPlacing;
        }
    }
}
