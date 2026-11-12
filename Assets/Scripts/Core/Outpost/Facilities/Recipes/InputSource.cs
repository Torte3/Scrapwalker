using System;
using System.Collections.Generic;
using Scrapwalker.Core.Items;
using Scrapwalker.Core.Root;

namespace Scrapwalker.Core.Outpost.Facilities.Recipes
{
    [Serializable]
    public abstract class InputSource
    {
        public abstract IReadOnlyList<MaterialAmount> Delivered(List<MaterialAmount> inputs);

        internal abstract bool IsReady(World world, Facility facility, WorkTask task);

        internal abstract bool TryBegin(World world, Facility facility, WorkTask task, int ticks);

        internal abstract bool IsOnTheWay(World world, Facility facility, WorkTask task);
    }
}
