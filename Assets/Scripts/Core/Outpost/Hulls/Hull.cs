using System.Collections.Generic;
using System.Linq;
using Scrapwalker.Core.Framework;
using Scrapwalker.Core.Outpost.Grid;
using Scrapwalker.Core.Outpost.Roles;
using Scrapwalker.Core.Root;

namespace Scrapwalker.Core.Outpost.Hulls
{
    public sealed class Hull
    {
        private const double Epsilon = 0.000001;

        private readonly World _world;
        private readonly List<Crack> _cracks = new List<Crack>();

        public Hull(World world)
        {
            _world = world;
        }

        public double CracksLost { get; internal set; }
        public int NextCrackID { get; internal set; } = 1;
        public IReadOnlyList<Crack> Cracks => _cracks;

        public int Max => _world.Configuration.HullMax + (int)_world.Facilities.SumOperational(FacilityStat.HullBonus);
        public double PerCrack => (double)Max / CrackLimit;
        public int HP => CeilingCount(Max * (1.0 - CracksLost / CrackLimit));

        private int CrackLimit => _world.Configuration.HullCracks;

        internal bool TakeHit(int amount, Cell hit)
        {
            if (amount <= 0 || HP <= 0)
            {
                return false;
            }

            CracksLost += amount / PerCrack;
            if (CracksLost > CrackLimit - Epsilon)
            {
                CracksLost = CrackLimit;
            }

            int want = CeilingCount(CracksLost);
            while (_cracks.Count < want)
            {
                AddCrack(hit);
            }

            return HP == 0;
        }

        private static int CeilingCount(double value)
        {
            return (int)System.Math.Ceiling(value - Epsilon);
        }

        internal Crack AddCrack(Cell hit)
        {
            var seen = new HashSet<Cell> { hit };
            var queue = new Queue<Cell>();
            queue.Enqueue(hit);
            while (true)
            {
                Cell cell = queue.Dequeue();
                if (CrackAt(cell) == null)
                {
                    var crack = new Crack(this, NextCrackID++, cell, _world.Configuration);
                    crack.Site.Time.Start(_world.Configuration.TicksFor(_world.Configuration.RepairSeconds));
                    _cracks.Add(crack);
                    return crack;
                }

                foreach (Cell next in Neighbors(cell))
                {
                    if (seen.Add(next))
                    {
                        queue.Enqueue(next);
                    }
                }
            }
        }

        private IEnumerable<Cell> Neighbors(Cell cell)
        {
            if (cell.Column > 0)
            {
                yield return new Cell(cell.Column - 1, cell.Row);
            }

            if (cell.Column < _world.Columns - 1)
            {
                yield return new Cell(cell.Column + 1, cell.Row);
            }

            if (cell.Row > 0)
            {
                yield return new Cell(cell.Column, cell.Row - 1);
            }

            if (cell.Row < _world.Rows - 1)
            {
                yield return new Cell(cell.Column, cell.Row + 1);
            }
        }

        public Crack CrackAt(Cell cell)
        {
            return _cracks.Find(crack => crack.Cell == cell);
        }

        public Crack FindCrack(int id)
        {
            return _cracks.Find(crack => crack.ID == id);
        }

        internal bool Repair(int id)
        {
            Crack crack = FindCrack(id);
            if (crack == null)
            {
                return false;
            }

            _cracks.Remove(crack);
            crack.IsRepaired = true;
            CracksLost = System.Math.Max(0.0, CracksLost - 1.0);
            return true;
        }

        internal void RestoreAfterDefeat()
        {
            CracksLost = CrackLimit - 1.0 / PerCrack;
        }

        internal HullMemento CreateMemento()
        {
            return new HullMemento { CracksLost = CracksLost, NextCrackID = NextCrackID, Cracks = _cracks.Select(crack => crack.CreateMemento()).ToList() };
        }

        internal void Restore(HullMemento memento, LoadContext load)
        {
            CracksLost = memento.CracksLost;
            NextCrackID = memento.NextCrackID;
            _cracks.Clear();
            _cracks.AddRange(memento.Cracks.Select(crack => Crack.Restore(this, crack, _world.Configuration, load)));
        }
    }
}
