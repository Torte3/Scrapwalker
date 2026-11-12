using System.Collections.Generic;
using System.Linq;
using Scrapwalker.Core.Root;

namespace Scrapwalker.Core.Outpost.Grid
{
    public sealed class Navigator
    {
        private const float Unreachable = 10000f;
        private const float OnRowTolerance = 0.01f;

        private readonly World _world;
        private readonly List<GridPoint> _costPath = new List<GridPoint>();

        public Navigator(World world)
        {
            _world = world;
        }

        public bool FindPath(GridPoint from, GridPoint to, List<GridPoint> into)
        {
            into.Clear();
            int row = (int)System.Math.Round(from.Y);
            int targetRow = (int)System.Math.Round(to.Y);
            float x = from.X;
            if (System.Math.Abs(from.Y - row) > OnRowTolerance)
            {
                into.Add(new GridPoint(from.X, row));
            }

            while (row != targetRow)
            {
                bool up = targetRow > row;
                Facility ladder = NearestLadder(up ? row : row - 1, x);
                if (ladder == null)
                {
                    into.Clear();
                    return false;
                }

                float ladderX = ladder.Cell.Column + 0.5f;
                into.Add(new GridPoint(ladderX, row));
                row += up ? 1 : -1;
                into.Add(new GridPoint(ladderX, row));
                x = ladderX;
            }

            into.Add(to);
            return true;
        }

        public bool CanReach(GridPoint from, GridPoint to)
        {
            return FindPath(from, to, _costPath);
        }

        public GridPoint NearestBetween(GridPoint from, GridPoint left, GridPoint right)
        {
            GridPoint onFloor = FindPath(from, left, _costPath) && _costPath.Count > 1 ? _costPath[_costPath.Count - 2] : from;
            return new GridPoint(System.Math.Clamp(onFloor.X, left.X, right.X), left.Y);
        }

        public float PathCost(GridPoint from, GridPoint to)
        {
            if (!FindPath(from, to, _costPath))
            {
                return Unreachable + from.StepsTo(to);
            }

            float cost = 0f;
            GridPoint at = from;
            foreach (GridPoint next in _costPath)
            {
                cost += at.StepsTo(next);
                at = next;
            }

            return cost;
        }

        public T Nearest<T>(IEnumerable<T> items, System.Func<T, bool> match, GridPoint from, System.Func<T, GridPoint> at)
            where T : class
        {
            T best = null;
            float bestDistance = float.MaxValue;
            foreach (T item in items.Where(match))
            {
                float d = PathCost(from, at(item));
                if (d < bestDistance)
                {
                    bestDistance = d;
                    best = item;
                }
            }

            return best;
        }

        private Facility NearestLadder(int row, float x)
        {
            return _world.Facilities
                .Where(f => f.IsOperational && f.Definition.ConnectsFloors && f.Cell.Row == row)
                .OrderBy(f => System.Math.Abs(f.Cell.Column + 0.5f - x))
                .FirstOrDefault();
        }
    }
}
