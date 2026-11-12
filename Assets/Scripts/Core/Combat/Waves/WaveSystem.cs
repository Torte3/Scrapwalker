using System;
using System.Collections.Generic;
using Scrapwalker.Core.Items;
using Scrapwalker.Core.Root;

namespace Scrapwalker.Core.Combat.Waves
{
    internal static class WaveSystem
    {
        internal static bool Tick(World world, Func<EnemyDefinition, IReadOnlyList<MaterialAmount>> drawDrops)
        {
            WaveState wave = world.Wave;
            if (IsCleared(world))
            {
                return true;
            }

            if (wave.Fight.CountDownOne())
            {
                wave.CancelSpawns();
                world.Battle.Clear();
                return true;
            }

            foreach (EnemyDefinition definition in wave.TakeDueSpawns())
            {
                world.Battle.AddEnemy(definition, world.Configuration.SpawnDistance, definition.HP, drawDrops(definition));
            }

            return false;
        }

        private static bool IsCleared(World world)
        {
            return world.Wave.PendingSpawns == 0 && world.Battle.CountAliveEnemies() == 0;
        }
    }
}
