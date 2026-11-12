using System.Linq;
using Scrapwalker.Core.Items;
using Scrapwalker.Core.Outpost;
using Scrapwalker.Core.Outpost.Facilities;

namespace Scrapwalker.Core.Research
{
    public sealed class ResearchWork
    {
        internal ResearchWork(ResearchDefinition research, WorkTask task, Facility facility)
        {
            Research = research;
            Task = task;
            Facility = facility;
        }

        public WorkTask Task { get; }
        public Facility Facility { get; }
        public ResearchDefinition Research { get; }

        public int DeliveredOf(MaterialDefinition type)
        {
            return Facility.Work.FedCount(Task, type);
        }

        public int DeliveredTotal => Task.Recipe.Inputs.Sum(a => System.Math.Min(Facility.Work.FedCount(Task, a.Type), a.Count));
    }
}
