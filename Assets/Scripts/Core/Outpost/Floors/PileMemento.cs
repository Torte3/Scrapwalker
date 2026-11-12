using Scrapwalker.Core.Items;
using Scrapwalker.Core.Outpost.Grid;

namespace Scrapwalker.Core.Outpost.Floors
{
    public sealed class PileMemento
    {
        public int ID;
        public FloorSlotMemento Slot;
        public PointMemento Spot;
        public StackMemento Items;
    }
}
