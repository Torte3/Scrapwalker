using Scrapwalker.Core.Items;
using Scrapwalker.Core.Outpost.Grid;
using Scrapwalker.Core.Root;

namespace Scrapwalker.Core.Outpost.Construction
{
    public sealed class PlaceFacility : Command
    {
        public PlaceFacility(FacilityDefinition definition, Cell at, QualityFilter? filter = null)
        {
            Definition = definition;
            At = at;
            Filter = filter ?? QualityFilter.Any;
        }

        public FacilityDefinition Definition { get; }
        public Cell At { get; }
        public QualityFilter Filter { get; }

        internal override bool IsAllowedBy(CommandRule rule, World world) => rule.AllowsPlacing(world, this);

        internal override string LogText => base.LogText + " " + Definition?.Key + " " + At;

        internal override CommandResult Check(World world)
        {
            if (Definition == null)
            {
                return CommandResult.Fail(CommandError.NoFacility);
            }

            CommandResult allowed = Definition.CheckAllowed(world);
            if (!allowed.IsOK)
            {
                return allowed;
            }

            CommandResult placement = CommandResult.From(GridRules.CheckPlacement(world, Definition.RectAt(At)));
            if (!placement.IsOK)
            {
                return placement;
            }

            return Filter.IsValid ? CommandResult.Success() : CommandResult.Fail(CommandError.BadRange);
        }

        protected override void Run(World world)
        {
            Facility facility = world.AddFacility(Definition, At);
            facility.Site.Filter = Filter;
            FacilityCondition.Place(world, facility);
        }
    }
}
