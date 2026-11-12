using System;
using Scrapwalker.Core.Outpost.Facilities.Recipes;

namespace Scrapwalker.Core.Outpost.Upgrades
{
    [Serializable]
    public abstract class UpgradeEffect
    {
        internal abstract void ApplyTo(OutpostTraits traits);

        public virtual void Describe(OutpostTraits before, IRecipeEffects effects)
        {
        }
    }
}
