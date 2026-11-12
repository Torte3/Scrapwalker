using System.Collections.Generic;
using Scrapwalker.Core.Combat;
using Scrapwalker.Core.Framework;
using Scrapwalker.Core.Items;
using Scrapwalker.Core.Outpost.Upgrades;

namespace Scrapwalker.Core.Travel
{
    public sealed class Departed : GameEvent
    {
        public Departed(RuinDefinition from, bool afterDefeat)
        {
            From = from;
            AfterDefeat = afterDefeat;
        }

        public RuinDefinition From { get; }
        public bool AfterDefeat { get; }
    }

    public sealed class DestinationChosen : GameEvent
    {
        public DestinationChosen(RuinDefinition ruin)
        {
            Ruin = ruin;
        }

        public RuinDefinition Ruin { get; }
    }

    public sealed class Arriving : GameEvent
    {
        public Arriving(RuinDefinition ruin)
        {
            Ruin = ruin;
        }

        public RuinDefinition Ruin { get; }
    }

    public sealed class Landed : GameEvent
    {
        public Landed(RuinDefinition ruin)
        {
            Ruin = ruin;
        }

        public RuinDefinition Ruin { get; }
    }

    public sealed class RuinsInSightCleared : GameEvent
    {
        public RuinsInSightCleared(IReadOnlyList<OutpostUpgradeDefinition> upgrades)
        {
            Upgrades = upgrades;
        }

        public IReadOnlyList<OutpostUpgradeDefinition> Upgrades { get; }
    }

    public sealed class RuinCompleted : GameEvent
    {
        public RuinCompleted(RuinDefinition ruin)
        {
            Ruin = ruin;
        }

        public RuinDefinition Ruin { get; }
    }

    public sealed class TookOff : GameEvent
    {
        public TookOff(RuinDefinition from, bool defeatedAll)
        {
            From = from;
            HasDefeatedAll = defeatedAll;
        }

        public RuinDefinition From { get; }
        public bool HasDefeatedAll { get; }
    }

    public sealed class EscapeChosen : GameEvent
    {
    }

    public sealed class TakeoffFinished : GameEvent
    {
    }

    public sealed class Looted : GameEvent
    {
        private Looted(LootSource source, int waveNumber, int enemyID, EnemyDefinition enemy, ItemStack items)
        {
            Source = source;
            WaveNumber = waveNumber;
            EnemyID = enemyID;
            Enemy = enemy;
            Items = items;
        }

        public static Looted Dug(int waveNumber, ItemStack items)
        {
            return new Looted(LootSource.Dig, waveNumber, 0, null, items);
        }

        public static Looted Dropped(int enemyID, EnemyDefinition enemy, int waveNumber, ItemStack items)
        {
            return new Looted(LootSource.Enemy, waveNumber, enemyID, enemy, items);
        }

        public LootSource Source { get; }
        public int WaveNumber { get; }
        public int EnemyID { get; }
        public EnemyDefinition Enemy { get; }
        public ItemStack Items { get; }
    }
}
