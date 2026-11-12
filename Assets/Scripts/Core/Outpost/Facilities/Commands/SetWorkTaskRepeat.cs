using Scrapwalker.Core.Root;

namespace Scrapwalker.Core.Outpost.Facilities.Commands
{
    public sealed class SetWorkTaskRepeat : WorkTaskCommand
    {
        public SetWorkTaskRepeat(int facilityID, int taskID, bool repeat)
            : base(facilityID, taskID)
        {
            Repeat = repeat;
        }

        public bool Repeat { get; }

        protected override CommandResult Check(World world, Facility facility, WorkTask task)
        {
            return task.Recipe.IsOneOff ? CommandResult.Fail(CommandError.OnceOnly) : CommandResult.Success();
        }

        protected override void Run(World world, Facility facility, WorkTask task)
        {
            task.IsRepeating = Repeat;
        }
    }
}
