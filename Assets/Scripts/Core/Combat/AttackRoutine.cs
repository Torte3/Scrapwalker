using System;
using System.Collections.Generic;
using Scrapwalker.Core.Framework;
using Scrapwalker.Core.Outpost.Grid;

namespace Scrapwalker.Core.Combat
{
    public sealed class AttackRoutine
    {
        private IReadOnlyList<Cell> _cells = Array.Empty<Cell>();

        public int Index { get; private set; }
        public ElapsedTicks Time { get; } = new ElapsedTicks();
        public int Fired { get; private set; }
        public Countdown RestTime { get; } = new Countdown();
        public IReadOnlyList<Cell> Cells => _cells;
        public bool IsInMove => RestTime.IsDone && _cells.Count > 0;

        internal void Restart(int index, int restTicks)
        {
            Index = index;
            Time.Reset();
            Fired = 0;
            RestTime.Start(restTicks);
            _cells = Array.Empty<Cell>();
        }

        internal void SetTargets(IReadOnlyList<Cell> cells)
        {
            _cells = cells;
        }

        internal int Advance(AttackMove move, float tickSeconds)
        {
            int from = Fired;
            Time.Tick();
            float elapsed = Time.Seconds(tickSeconds);
            while (Fired < move.ShotCount && elapsed >= move.ImpactAt(Fired))
            {
                Fired++;
            }

            return from;
        }

        internal bool IsDone(AttackMove move, float tickSeconds)
        {
            return Time.Seconds(tickSeconds) >= move.Duration;
        }
    }
}
