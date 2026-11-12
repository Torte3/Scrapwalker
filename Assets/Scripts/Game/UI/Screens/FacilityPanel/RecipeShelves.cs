using System.Collections.Generic;
using Scrapwalker.Core.Outpost.Facilities;
using Scrapwalker.Core.Outpost.Facilities.Recipes;

namespace Scrapwalker.Game.UI.Screens.FacilityPanel
{
    internal sealed class RecipeShelves : IRecipeShelfView<List<RecipeDefinition>>
    {
        private readonly List<RecipeDefinition> _unshelved = new List<RecipeDefinition>();

        public List<RecipeDefinition> Tasks { get; } = new List<RecipeDefinition>();
        public List<RecipeDefinition> Upgrades { get; } = new List<RecipeDefinition>();

        public void Sort(IEnumerable<RecipeDefinition> recipes)
        {
            Tasks.Clear();
            Upgrades.Clear();
            foreach (RecipeDefinition recipe in recipes)
            {
                recipe.Result.ShowShelf(this).Add(recipe);
            }

            _unshelved.Clear();
        }

        public List<RecipeDefinition> Task() => Tasks;

        public List<RecipeDefinition> Upgrade() => Upgrades;

        public List<RecipeDefinition> Research() => _unshelved;
    }
}
