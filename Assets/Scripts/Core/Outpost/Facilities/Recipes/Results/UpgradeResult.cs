using System;
using Scrapwalker.Core.Root;

namespace Scrapwalker.Core.Outpost.Facilities.Recipes.Results
{
    [Serializable]
    public class UpgradeResult : RecipeResult
    {
        public override T ShowShelf<T>(IRecipeShelfView<T> view)
        {
            return view.Upgrade();
        }

        public override bool WorksOnRoom => true;

        public override bool IsSpent(World world, Facility facility, RecipeDefinition recipe)
        {
            return facility.Work.Upgrades.Has(recipe.ID);
        }

        public override void Describe(World world, Facility facility, RecipeDefinition recipe, IRecipeEffects effects)
        {
            foreach (RecipeDefinition unlocked in facility.Definition.Recipes)
            {
                if (unlocked.IsOpenedBy(recipe))
                {
                    effects.Unlock(unlocked, unlocked.PendingResearch(world));
                }
            }
        }

        internal override void Finish(World world, Facility facility, WorkJob job)
        {
            facility.Work.Upgrades.Add(job.Task.Recipe.ID);
        }
    }
}
