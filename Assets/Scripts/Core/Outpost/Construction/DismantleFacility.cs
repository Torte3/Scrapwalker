using Scrapwalker.Core.Root;

namespace Scrapwalker.Core.Outpost.Construction
{
    public sealed class DismantleFacility : FacilityCommand
    {
        public DismantleFacility(int facilityID)
            : base(facilityID)
        {
        }

        protected override CommandResult Check(World world, Facility facility)
        {
            return CommandResult.From(facility.Condition.Refusal(FacilityOrder.Dismantle));
        }

        protected override void Run(World world, Facility facility)
        {
            facility.Condition.Dismantle(world, facility);
        }
    }
}
