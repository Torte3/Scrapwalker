using System;
using System.Collections.Generic;
using Scrapwalker.Core.Framework;
using Scrapwalker.Core.Items;

namespace Scrapwalker.Core.Combat.Waves
{
    [Serializable]
    public sealed class WaveDefinition
    {
        public List<WaveSpawn> Enemies = new List<WaveSpawn>();
        public DefinitionReference<ItemTableDefinition> LootReference = new DefinitionReference<ItemTableDefinition>();
        public float RetreatSeconds;

        public ItemTableDefinition Loot
        {
            get => LootReference?.Definition;
            set => LootReference = new DefinitionReference<ItemTableDefinition>(value);
        }
    }
}
