using System;
using System.Collections.Generic;
using Scrapwalker.Core.Framework;
using Scrapwalker.Core.Items;

namespace Scrapwalker.Core.Combat
{
    [Serializable]
    public sealed class EnemyDefinition : Definition
    {
        public DefinitionReference<EnemyTypeDefinition> TypeReference = new DefinitionReference<EnemyTypeDefinition>();
        public int HP = 1;
        public int Armor;
        public bool IsBoss;
        public List<AttackMove> AttackMoves = new List<AttackMove>();
        public DefinitionReference<ItemTableDefinition> DropsReference = new DefinitionReference<ItemTableDefinition>();

        public EnemyTypeDefinition Type
        {
            get => TypeReference?.Definition;
            set => TypeReference = new DefinitionReference<EnemyTypeDefinition>(value);
        }

        public ItemTableDefinition Drops
        {
            get => DropsReference?.Definition;
            set => DropsReference = new DefinitionReference<ItemTableDefinition>(value);
        }

        public bool HasAttacks => AttackMoves.Count > 0;

        public int NextMove()
        {
            if (AttackMoves.Count == 1)
            {
                return 0;
            }

            int index = GameRandom.PickIndex(AttackMoves, m => m.Weight);
            return index >= 0 ? index : GameRandom.Range(0, AttackMoves.Count);
        }
    }
}
