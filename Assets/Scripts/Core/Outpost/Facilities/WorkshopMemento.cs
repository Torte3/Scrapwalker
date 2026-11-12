using System.Collections.Generic;
using Scrapwalker.Core.Items;

namespace Scrapwalker.Core.Outpost.Facilities
{
    public sealed class WorkshopMemento
    {
        public QueueMemento Queue;
        public List<StackMemento> Inputs = new List<StackMemento>();
        public int? WorkingTask;
        public JobMemento Job;
        public List<string> Upgrades = new List<string>();
    }
}
