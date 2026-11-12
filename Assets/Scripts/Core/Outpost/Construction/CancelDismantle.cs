using Scrapwalker.Core.Root;

namespace Scrapwalker.Core.Outpost.Construction
{
    public sealed class CancelDismantle : FacilityCommand
    {
        public CancelDismantle(int facilityID)
            : base(facilityID)
        {
        }

        protected override CommandResult Check(World world, Facility facility)
        {
            return CommandResult.From(facility.Condition.Refusal(FacilityOrder.CancelDismantle));
        }

        protected override void Run(World world, Facility facility)
        {
            facility.Condition.CancelDismantle(world, facility);
        }
    }
}
