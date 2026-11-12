using Scrapwalker.Core.Items;
using Scrapwalker.Core.Outpost.Floors;
using Scrapwalker.Core.Root;

namespace Scrapwalker.Core.Outpost.Facilities.Commands
{
    public sealed class SetInputFilter : WorkTaskCommand
    {
        public SetInputFilter(int facilityID, int taskID, QualityFilter filter)
            : base(facilityID, taskID)
        {
            Filter = filter;
        }

        public QualityFilter Filter { get; }

        protected override CommandResult Check(World world, Facility facility, WorkTask task)
        {
            return Filter.IsValid ? CommandResult.Success() : CommandResult.Fail(CommandError.BadRange);
        }

        protected override void Run(World world, Facility facility, WorkTask task)
        {
            task.Filter = Filter;
            SpillSystem.SpillUnneededInputs(world, facility);
        }
    }
}
