using System.Collections.Generic;

namespace Scrapwalker.Core.Outpost.Facilities
{
    public sealed class QueueMemento
    {
        public int NextTaskID;
        public List<TaskMemento> Tasks = new List<TaskMemento>();
    }
}
