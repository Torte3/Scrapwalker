using System.Collections.Generic;
using Scrapwalker.Core.Framework;
using Scrapwalker.Core.Items;

namespace Scrapwalker.Core.Outpost.Construction
{
    public sealed class WorksiteMemento
    {
        public List<StackMemento> Delivered = new List<StackMemento>();
        public FilterMemento Filter;
        public CountdownMemento Time;
    }
}
