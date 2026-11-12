using System;
using Scrapwalker.Core.Root;

namespace Scrapwalker.Core.Outpost.Facilities.Commands
{
    public sealed class MoveWorkTask : WorkTaskCommand
    {
        public MoveWorkTask(int facilityID, int taskID, MoveDirection direction)
            : base(facilityID, taskID)
        {
            Direction = direction;
        }

        public MoveDirection Direction { get; }

        protected override CommandResult Check(World world, Facility facility, WorkTask task)
        {
            WorkQueue queue = facility.Work.Queue;
            int from = queue.IndexOf(task);
            int to = Destination(facility, from);
            if (to < 0 || to >= queue.Count || to == from)
            {
                return CommandResult.Fail(CommandError.CannotMoveFurther);
            }

            if (PassesRunningTask(facility, Math.Min(from, to), Math.Max(from, to)))
            {
                return CommandResult.Fail(CommandError.CannotMoveFurther);
            }

            return CommandResult.Success();
        }

        private static bool PassesRunningTask(Facility facility, int first, int last)
        {
            WorkTask running = facility.Work.WorkingTask;
            int at = running == null ? -1 : facility.Work.Queue.IndexOf(running);
            return at >= 0 && first <= at && at <= last;
        }

        protected override void Run(World world, Facility facility, WorkTask task)
        {
            int from = facility.Work.Queue.IndexOf(task);
            facility.Work.Queue.Move(from, Destination(facility, from));
        }

        private int Destination(Facility facility, int from)
        {
            switch (Direction)
            {
                case MoveDirection.Up:
                    return from - 1;
                case MoveDirection.Down:
                    return from + 1;
                case MoveDirection.Front:
                    return FrontOf(facility);
                default:
                    throw new ArgumentOutOfRangeException(nameof(Direction), Direction, null);
            }
        }

        private static int FrontOf(Facility facility)
        {
            WorkTask running = facility.Work.WorkingTask;
            return running == null ? 0 : facility.Work.Queue.IndexOf(running) + 1;
        }
    }
}
