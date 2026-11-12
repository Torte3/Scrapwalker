using System.Collections.Generic;
using Scrapwalker.Core.Outpost.Bots.Actions;
using Scrapwalker.Core.Outpost.Floors;
using Scrapwalker.Core.Root;

namespace Scrapwalker.Core.Outpost.Bots.Assignments
{
    public sealed class FloorAssignment : Assignment
    {
        public override IEnumerable<AssignmentAction> Actions(World world)
        {
            if (!world.Stock.HasFreeSpace)
            {
                yield break;
            }

            foreach (Pile pile in world.Floor.Piles)
            {
                if (!world.IsTaking(Place.AtPile(pile)))
                {
                    yield return new PickupAction(WorkPriority.Floor, pile, Place.Storage, pile.Type, null, pile.Count);
                }
            }
        }
    }
}
