using Scrapwalker.Core.Root;

namespace Scrapwalker.Core.Outpost.Construction
{
    public sealed class CancelPlacement : FacilityCommand
    {
        public CancelPlacement(int facilityID)
            : base(facilityID)
        {
        }

        protected override CommandResult Check(World world, Facility facility)
        {
            return CommandResult.From(facility.Condition.Refusal(FacilityOrder.CancelPlacement));
        }

        protected override void Run(World world, Facility facility)
        {
            facility.Condition.CancelPlacement(world, facility);
        }
    }
}
