using Scrapwalker.Core.Items;
using Scrapwalker.Core.Outpost;
using Scrapwalker.Core.Outpost.Construction;
using Scrapwalker.Core.Outpost.Grid;
using Scrapwalker.Core.Root;
using Scrapwalker.Game.Wording.Words;

namespace Scrapwalker.Game.UI.Build
{
    public sealed class RelocateGesture : BuildGesture
    {
        private readonly Facility _facility;

        public RelocateGesture(Facility facility)
        {
            _facility = facility;
        }

        public override FacilityDefinition Definition => _facility.Definition;
        public override Facility Relocating => _facility;
        internal override string ClickText => WorldWords.ClickRelocate;
        public override bool EndsOnDeselect => true;
        public override bool DeselectsAfterRun => true;

        internal override Command CommandAt(Cell at, QualityFilter filter)
        {
            return new RelocateFacility(_facility.ID, at);
        }
    }
}
