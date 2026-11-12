using Scrapwalker.Core.Outpost.Floors;
using Scrapwalker.Core.Root;

namespace Scrapwalker.Core.Outpost.Facilities.Commands
{
    public sealed class RemoveWorkTask : WorkTaskCommand
    {
        public RemoveWorkTask(int facilityID, int taskID)
            : base(facilityID, taskID)
        {
        }

        protected override void Run(World world, Facility facility, WorkTask task)
        {
            Remove(world, facility, task);
        }

        internal static void Remove(World world, Facility facility, WorkTask task)
        {
            facility.Work.Queue.Remove(task);
            if (facility.Work.WorkingTask == task)
            {
                SpillSystem.SpillWork(world, facility);
            }

            SpillSystem.SpillHeldJob(world, facility, task);

            SpillSystem.SpillUnneededInputs(world, facility);
        }
    }
}
