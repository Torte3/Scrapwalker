using Scrapwalker.Core.Combat;
using Scrapwalker.Core.Framework;
using Scrapwalker.Core.Items;
using Scrapwalker.Core.Root;

namespace Scrapwalker.Core.Travel
{
    internal static class SalvageSystem
    {
        internal static void Listen(World world)
        {
            world.Events.On<EnemyKilled>(killed => CollectDrops(world, killed));
        }

        private static void CollectDrops(World world, EnemyKilled killed)
        {
            foreach (MaterialAmount drop in killed.Drops)
            {
                CollectLoot(world, world.Journey.CurrentRuin, drop, items => Looted.Dropped(killed.EnemyID, killed.Enemy, world.Wave.Number, items));
            }
        }

        internal static void CollectWaveLoot(World world, RuinDefinition ruin, int waveNumber)
        {
            foreach (MaterialAmount dug in Roll(world, ruin.WaveAt(waveNumber).Loot))
            {
                CollectLoot(world, ruin, dug, items => Looted.Dug(waveNumber, items));
            }
        }

        private static MaterialAmount[] Roll(World world, ItemTableDefinition table)
        {
            return table != null ? table.Roll(world.Definitions.Materials) : System.Array.Empty<MaterialAmount>();
        }

        internal static MaterialAmount[] DrawDrops(World world, EnemyDefinition enemy)
        {
            return Roll(world, enemy.Drops);
        }

        internal static void CollectLoot(World world, RuinDefinition ruin, MaterialAmount loot, System.Func<ItemStack, Looted> looted)
        {
            var byQuality = new int[QualityRules.Count];
            for (int i = 0; i < loot.Count; i++)
            {
                byQuality[(int)RollQuality(world, ruin)]++;
            }

            for (int q = byQuality.Length - 1; q >= 0; q--)
            {
                if (byQuality[q] > 0)
                {
                    world.Emit(looted(ItemStack.Of(loot.Type, (Quality)q, byQuality[q])));
                }
            }
        }

        internal static Quality RollQuality(World world, RuinDefinition ruin)
        {
            if (ruin == null)
            {
                return Quality.Plain;
            }

            int index = GameRandom.PickIndex(ruin.QualityWeights, w => w);
            return index >= 0 ? (Quality)index : Quality.Plain;
        }
    }
}
