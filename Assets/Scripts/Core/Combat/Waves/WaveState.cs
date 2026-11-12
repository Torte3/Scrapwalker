using System.Collections.Generic;
using Scrapwalker.Core.Framework;
using Scrapwalker.Core.Root;

namespace Scrapwalker.Core.Combat.Waves
{
    public sealed class WaveState
    {
        private readonly List<QueuedEnemy> _spawns = new List<QueuedEnemy>();
        private int _nextSpawn;

        public int Number { get; private set; }
        public Countdown Fight { get; } = new Countdown();
        public bool HasBoss => _spawns.Exists(spawn => spawn.Definition.IsBoss);
        public bool TimedOut => Fight.IsDone;
        public float RetreatSeconds { get; private set; }
        public int PendingSpawns => _spawns.Count - _nextSpawn;

        public float SecondsLeft(GameConfiguration configuration)
        {
            return Fight.Remaining > 0 ? Fight.Remaining * configuration.TickSeconds : 0f;
        }

        internal void Reset()
        {
            Number = 0;
            Fight.Start(0);
            RetreatSeconds = 0f;
            _spawns.Clear();
            _nextSpawn = 0;
        }

        internal void StartFighting(int number, IEnumerable<QueuedEnemy> spawns, float retreatSeconds, GameConfiguration configuration)
        {
            Reset();
            Number = number;
            RetreatSeconds = retreatSeconds;
            Fight.Start(configuration.TicksFor(retreatSeconds));
            _spawns.AddRange(spawns);
        }

        internal IEnumerable<EnemyDefinition> TakeDueSpawns()
        {
            while (_nextSpawn < _spawns.Count && _spawns[_nextSpawn].AtTick < Fight.Elapsed)
            {
                yield return _spawns[_nextSpawn++].Definition;
            }
        }

        internal void CancelSpawns()
        {
            _nextSpawn = _spawns.Count;
        }
    }
}
