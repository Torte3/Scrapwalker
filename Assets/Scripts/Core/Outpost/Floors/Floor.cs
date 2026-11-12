using System.Collections.Generic;
using System.Linq;
using Scrapwalker.Core.Framework;
using Scrapwalker.Core.Items;
using Scrapwalker.Core.Outpost.Grid;
using Scrapwalker.Core.Root;

namespace Scrapwalker.Core.Outpost.Floors
{
    public sealed class Floor
    {
        private const float ColumnWeight = 8f;
        private const float RowWeight = 10f;

        private readonly World _world;
        private readonly List<Pile> _piles = new List<Pile>();
        private int _nextPileID = 1;

        public Floor(World world)
        {
            _world = world;
        }

        public IReadOnlyList<Pile> Piles => _piles;

        internal bool TryTake(Pile pile, int count, out ItemStack taken)
        {
            taken = default;
            if (!_piles.Remove(pile))
            {
                return false;
            }

            int take = count < pile.Count ? count : pile.Count;
            if (take < pile.Count)
            {
                _piles.Add(new Pile(pile, pile.Items.WithCount(pile.Count - take)));
            }

            taken = pile.Items.WithCount(take);
            return true;
        }

        internal List<PileMemento> CreateMemento()
        {
            return _piles.Select(pile => pile.CreateMemento()).ToList();
        }

        internal void Restore(List<PileMemento> memento, LoadContext load)
        {
            _piles.Clear();
            _piles.AddRange(memento.Select(pile => Pile.Restore(pile, load)));
            _nextPileID = _piles.Count > 0 ? _piles.Max(pile => pile.ID) + 1 : 1;
        }

        internal Pile Find(int id)
        {
            return _piles.Find(pile => pile.ID == id);
        }

        internal Pile Drop(GridPoint at, ItemStack items)
        {
            if (items.IsEmpty)
            {
                return null;
            }

            Cell near = CellNear(at);
            int limit = items.Type.StackLimit;
            int left = items.Count;
            Pile last = null;
            while (left > 0)
            {
                FloorSlot? slot = FindSlot(items, at, near, limit, out int held);
                if (!slot.HasValue)
                {
                    _world.Log.Write(() => "床に置き場所が無く消えた " + items.WithCount(left).LogText);
                    break;
                }

                int put = System.Math.Min(left, limit - held);
                last = held > 0 ? PileAt(slot.Value) : null;
                if (last != null)
                {
                    last.Grow(put);
                }
                else
                {
                    last = new Pile(_nextPileID++, slot.Value, slot.Value.SpotAmong(_world.Configuration.FloorSlotsPerCell), items.WithCount(put));
                    _piles.Add(last);
                }

                left -= put;
            }

            return last;
        }

        private Cell CellNear(GridPoint at)
        {
            int column = System.Math.Clamp((int)System.Math.Floor(at.X), 0, _world.Columns - 1);
            int row = System.Math.Clamp((int)System.Math.Round(at.Y), 0, _world.Rows - 1);
            return new Cell(column, row);
        }

        private FloorSlot? FindSlot(ItemStack items, GridPoint at, Cell near, int limit, out int held)
        {
            FloorSlot? merge = null;
            FloorSlot? free = null;
            float bestMerge = float.MaxValue;
            float bestFree = float.MaxValue;
            int mergeHeld = 0;
            int slots = _world.Configuration.FloorSlotsPerCell;
            foreach (FloorSlot slot in Slots())
            {
                Pile here = PileAt(slot);
                float d = System.Math.Abs(slot.SpotAmong(slots).X - at.X) * ColumnWeight + System.Math.Abs(slot.Cell.Row - near.Row) * RowWeight;
                if (here == null)
                {
                    if (IsCloser(d, slot, bestFree, free, near))
                    {
                        bestFree = d;
                        free = slot;
                    }
                }
                else if (here.Items.IsSameItemAs(items) && here.Count < limit && IsCloser(d, slot, bestMerge, merge, near))
                {
                    bestMerge = d;
                    merge = slot;
                    mergeHeld = here.Count;
                }
            }

            held = merge.HasValue ? mergeHeld : 0;
            return merge ?? free;
        }

        private static bool IsCloser(float d, FloorSlot slot, float bestD, FloorSlot? best, Cell near)
        {
            if (d != bestD)
            {
                return d < bestD;
            }

            return best.HasValue && slot.Cell == near && best.Value.Cell != near;
        }

        private IEnumerable<FloorSlot> Slots()
        {
            for (int r = 0; r < _world.Rows; r++)
            {
                for (int c = 0; c < _world.Columns; c++)
                {
                    for (int s = 0; s < _world.Configuration.FloorSlotsPerCell; s++)
                    {
                        yield return new FloorSlot(new Cell(c, r), s);
                    }
                }
            }
        }

        private Pile PileAt(FloorSlot slot)
        {
            return _piles.Find(p => p.Slot.Equals(slot));
        }
    }
}
