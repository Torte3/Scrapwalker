using Scrapwalker.Core.Outpost;
using Scrapwalker.Core.Outpost.Grid;
using Scrapwalker.Game.MainView.Outpost;

namespace Scrapwalker.Game.MainView.Compose
{
    internal sealed class FacilityJobReader : IFacilityConditionView<FacilityJob>
    {
        private Facility _facility;

        public FacilityJob Read(Facility facility)
        {
            _facility = facility;
            return facility.Condition.Show(this);
        }

        public FacilityJob Planned() => FacilityJob.Building;

        public FacilityJob Rebuilding() => FacilityJob.Rebuilding;

        public FacilityJob Reassembling() => FacilityJob.Reassembling;

        public FacilityJob Operational() => _facility.Work.IsUpgrading ? FacilityJob.Upgrading : FacilityJob.None;

        public FacilityJob Dismantling() => FacilityJob.Dismantling;

        public FacilityJob Relocating(Cell target) => FacilityJob.Dismantling;
    }
}
