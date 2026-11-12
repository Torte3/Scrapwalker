using Scrapwalker.Core.Outpost.Grid;

namespace Scrapwalker.Core.Outpost.Floors
{
    public readonly struct FloorSlot : System.IEquatable<FloorSlot>
    {
        public FloorSlot(Cell cell, int index)
        {
            Cell = cell;
            Index = index;
        }

        public Cell Cell { get; }
        public int Index { get; }

        internal FloorSlotMemento CreateMemento()
        {
            return new FloorSlotMemento { Cell = Cell.CreateMemento(), Index = Index };
        }

        internal static FloorSlot Restore(FloorSlotMemento memento)
        {
            return new FloorSlot(Grid.Cell.Restore(memento.Cell), memento.Index);
        }

        public GridPoint SpotAmong(int slots)
        {
            return new GridPoint(Cell.Column + (Index + 0.5f) / slots, Cell.Row);
        }

        public bool Equals(FloorSlot other)
        {
            return Cell == other.Cell && Index == other.Index;
        }

        public override bool Equals(object value)
        {
            return value is FloorSlot other && Equals(other);
        }

        public override int GetHashCode()
        {
            return Cell.GetHashCode() * 31 + Index;
        }
    }
}
