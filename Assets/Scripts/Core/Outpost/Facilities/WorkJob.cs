using Scrapwalker.Core.Framework;
using Scrapwalker.Core.Items;
using Scrapwalker.Core.Outpost.Facilities.Recipes.Results;

namespace Scrapwalker.Core.Outpost.Facilities
{
    public sealed class WorkJob
    {
        internal WorkJob(WorkTask task, int ticksTotal, Quality? inputQuality)
        {
            Task = task;
            Time.Start(ticksTotal);
            InputQuality = inputQuality;
        }

        public WorkTask Task { get; }
        public StackList Lots { get; } = new StackList();
        public Countdown Time { get; } = new Countdown();
        public Quality? InputQuality { get; }
        public Volley Volley { get; internal set; }

        public float Progress => Time.Progress;

        internal JobMemento CreateMemento()
        {
            return new JobMemento
            {
                Time = Time.CreateMemento(),
                Input = InputQuality,
                Volley = Volley?.CreateMemento(),
                Lots = Lots.CreateMemento(),
            };
        }

        internal static WorkJob Restore(JobMemento memento, WorkTask task, LoadContext load)
        {
            var job = new WorkJob(task, memento.Time.Total, memento.Input)
            {
                Volley = Volley.Restore(memento.Volley),
            };
            job.Time.Restore(memento.Time);
            job.Lots.Restore(memento.Lots, load);
            return job;
        }

        internal static WorkJob Start(WorkTask task, StackList stacks, int ticks)
        {
            var job = new WorkJob(task, ticks, stacks.AverageQuality);
            job.Lots.AddRange(stacks.Stacks);
            return job;
        }
    }
}
