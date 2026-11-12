using System;
using Scrapwalker.Core.Outpost.Facilities.Recipes;

namespace Scrapwalker.Core.Outpost.Upgrades
{
    [Serializable]
    public sealed class BotLimitEffect : UpgradeEffect
    {
        public int Limit;

        internal override void ApplyTo(OutpostTraits traits)
        {
            traits.BotLimit = Math.Max(traits.BotLimit, Limit);
        }

        public override void Describe(OutpostTraits before, IRecipeEffects effects)
        {
            effects.BotLimit(before.BotLimit, Limit);
        }
    }
}
