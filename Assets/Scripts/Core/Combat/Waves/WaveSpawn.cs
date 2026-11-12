using System;
using Scrapwalker.Core.Framework;

namespace Scrapwalker.Core.Combat.Waves
{
    [Serializable]
    public struct WaveSpawn
    {
        public DefinitionReference<EnemyDefinition> EnemyReference;
        public float At;

        public EnemyDefinition Enemy => EnemyReference?.Definition;
    }
}
