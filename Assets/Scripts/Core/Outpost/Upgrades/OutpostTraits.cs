using System.Collections.Generic;
using Scrapwalker.Core.Root;

namespace Scrapwalker.Core.Outpost.Upgrades
{
    public sealed class OutpostTraits
    {
        public int BotLimit { get; internal set; }
        public TimeOfDay TimeOfDay { get; internal set; }

        internal static OutpostTraits Of(IEnumerable<UpgradeEffect> start, IEnumerable<OutpostUpgradeDefinition> upgrades)
        {
            var traits = new OutpostTraits();
            foreach (UpgradeEffect effect in start)
            {
                effect.ApplyTo(traits);
            }

            foreach (OutpostUpgradeDefinition upgrade in upgrades)
            {
                foreach (UpgradeEffect effect in upgrade.Effects)
                {
                    effect.ApplyTo(traits);
                }
            }

            return traits;
        }
    }
}
