using System;
using Scrapwalker.Core.Outpost.Roles;
using Scrapwalker.Core.Root;

namespace Scrapwalker.Core.Outpost.Facilities.Recipes.Results
{
    [Serializable]
    public sealed class StatUpgradeResult : UpgradeResult
    {
        public StatAmount Raise;

        internal override float Adds(FacilityStat stat)
        {
            return Raise.Of(stat);
        }

        public override void Check(FacilityDefinition facility, RecipeDefinition recipe, IRecipeCheck check)
        {
            if (Raise.Amount <= 0f)
            {
                check.BadRecipe();
            }
        }

        public override void Describe(World world, Facility facility, RecipeDefinition recipe, IRecipeEffects effects)
        {
            float from = facility.Definition.StatBefore(Raise.Stat, recipe);
            float to = from + Raise.Amount;
            bool described = false;
            foreach (RecipeDefinition other in facility.Definition.Recipes)
            {
                described |= other.Result.DescribeChange(world, facility, other, Raise.Stat, from, to, effects);
            }

            if (!described)
            {
                effects.Change(Raise.Stat, from, to);
            }

            base.Describe(world, facility, recipe, effects);
        }
    }
}
