using System;
using System.Collections.Generic;
using System.Linq;
using Scrapwalker.Core.Items;
using Scrapwalker.Core.Root;

namespace Scrapwalker.Core.Outpost.Facilities.Recipes
{
    [Serializable]
    public sealed class StoreInputs : InputSource
    {
        public override IReadOnlyList<MaterialAmount> Delivered(List<MaterialAmount> inputs)
        {
            return Array.Empty<MaterialAmount>();
        }

        internal override bool IsReady(World world, Facility facility, WorkTask task)
        {
            return MaterialAmount.PerMaterial(task.Recipe.Inputs)
                .All(amount => world.Ledger.Available(amount.Type, task.Filter) >= amount.Count);
        }

        internal override bool TryBegin(World world, Facility facility, WorkTask task, int ticks)
        {
            if (!IsReady(world, facility, task))
            {
                return false;
            }

            var stacks = new StackList();
            foreach (MaterialAmount amount in task.Recipe.Inputs)
            {
                for (int i = 0; i < amount.Count; i++)
                {
                    if (!world.Ledger.TryTakeOne(amount.Type, task.Filter, facility.Door, out Quality quality))
                    {
                        break;
                    }

                    stacks.Add(ItemStack.Of(amount.Type, quality, 1));
                }
            }

            facility.Work.Begin(task, stacks, ticks);
            return true;
        }

        internal override bool IsOnTheWay(World world, Facility facility, WorkTask task)
        {
            return false;
        }
    }
}
