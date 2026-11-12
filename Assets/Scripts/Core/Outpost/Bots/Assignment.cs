using System.Collections.Generic;
using Scrapwalker.Core.Root;

namespace Scrapwalker.Core.Outpost.Bots
{
    public abstract class Assignment
    {
        public abstract IEnumerable<AssignmentAction> Actions(World world);
    }
}
