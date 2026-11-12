using Scrapwalker.Core.Outpost.Grid;
using Scrapwalker.Core.Root;

namespace Scrapwalker.Core.Outpost.Construction
{
    public sealed class RelocateFacility : FacilityCommand
    {
        public RelocateFacility(int facilityID, Cell to)
            : base(facilityID)
        {
            To = to;
        }

        public Cell To { get; }

        internal override string LogText => base.LogText + " → " + To;

        protected override CommandResult Check(World world, Facility facility)
        {
            CommandError refusal = facility.Condition.Refusal(FacilityOrder.Relocate);
            return refusal != CommandError.None ? CommandResult.Fail(refusal) : CommandResult.From(GridRules.CheckPlacement(world, facility.Definition.RectAt(To), facility));
        }

        protected override void Run(World world, Facility facility)
        {
            facility.Condition.Relocate(world, facility, To);
        }
    }
}
