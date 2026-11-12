using System.Collections.Generic;
using Scrapwalker.Core.Framework;
using Scrapwalker.Core.Items;
using Scrapwalker.Core.Outpost.Facilities.Recipes.Results;

namespace Scrapwalker.Core.Outpost.Facilities
{
    public sealed class JobMemento
    {
        public CountdownMemento Time;
        public Quality? Input;
        public VolleyMemento Volley;
        public List<StackMemento> Lots = new List<StackMemento>();
    }
}
