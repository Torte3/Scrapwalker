using System;
using System.Collections.Generic;
using Scrapwalker.Core.Items;
using Scrapwalker.Core.Root;

namespace Scrapwalker.Core.Outpost.Facilities.Recipes
{
    [Serializable]
    public sealed class DeliveredInputs : InputSource
    {
        public override IReadOnlyList<MaterialAmount> Delivered(List<MaterialAmount> inputs)
        {
            return inputs;
        }

        internal override bool IsReady(World world, Facility facility, WorkTask task)
        {
            return facility.Work.HasInputsFor(task);
        }

        internal override bool TryBegin(World world, Facility facility, WorkTask task, int ticks)
        {
            return facility.Work.TryBeginWithInputs(task, ticks);
        }

        internal override bool IsOnTheWay(World world, Facility facility, WorkTask task)
        {
            return world.Ledger
                .Shortage(task.Recipe.Inputs, task.Filter, type => facility.Work.FedCount(task, type))
                .Count == 0;
        }
    }
}
