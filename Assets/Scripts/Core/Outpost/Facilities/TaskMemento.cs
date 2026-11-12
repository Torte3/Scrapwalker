using Scrapwalker.Core.Items;

namespace Scrapwalker.Core.Outpost.Facilities
{
    public sealed class TaskMemento
    {
        public int ID;
        public string Recipe;
        public bool Repeat;
        public FilterMemento Filter;
        public JobMemento Held;
    }
}
