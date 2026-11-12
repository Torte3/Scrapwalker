using Scrapwalker.Core.Root;

namespace Scrapwalker.Core.Outpost.Facilities.Commands
{
    public abstract class WorkTaskCommand : FacilityCommand
    {
        protected WorkTaskCommand(int facilityID, int taskID)
            : base(facilityID)
        {
            TaskID = taskID;
        }

        public int TaskID { get; }

        internal override string LogText => base.LogText + " " + WorkTask.LogNameOf(TaskID);

        protected sealed override CommandResult Check(World world, Facility facility)
        {
            WorkTask task = facility.Work.Queue.Find(TaskID);
            return task == null ? CommandResult.Fail(CommandError.NoSuchTask) : Check(world, facility, task);
        }

        protected sealed override void Run(World world, Facility facility)
        {
            Run(world, facility, facility.Work.Queue.Find(TaskID));
        }

        protected virtual CommandResult Check(World world, Facility facility, WorkTask task)
        {
            return CommandResult.Success();
        }

        protected abstract void Run(World world, Facility facility, WorkTask task);
    }
}
