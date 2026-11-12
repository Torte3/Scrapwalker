using Scrapwalker.Core.Outpost;
using Scrapwalker.Core.Outpost.Facilities;
using Scrapwalker.Core.Outpost.Facilities.Commands;
using Scrapwalker.Core.Root;

namespace Scrapwalker.Core.Sessions.Tutorial.Goals
{
    public sealed class OrderRecipeGoal : FacilityGoal
    {
        private readonly string _recipe;

        internal OrderRecipeGoal(string facility, string recipe) : base(facility)
        {
            _recipe = recipe;
        }

        internal override bool AllowsTask(World world, AddWorkTask task)
        {
            return RecipeOf(world, task) == RecipeIn(world);
        }

        public override bool Reached(World world)
        {
            RecipeDefinition recipe = RecipeIn(world);
            return recipe != null && world.Facilities.FindTask(r => r == recipe, out Facility _) != null;
        }

        public override T Guide<T>(World world, ITutorialGuide<T> guide)
        {
            return guide.Order(FacilityOf(world), RecipeIn(world), 1, null);
        }

        public RecipeDefinition RecipeIn(World world)
        {
            return FacilityOf(world).FindRecipe(_recipe);
        }

        protected override void CheckIn(FacilityDefinition facility, GameDefinitions definitions, ITutorialGoalCheck check)
        {
            if (facility.FindRecipe(_recipe) == null)
            {
                check.MissingRecipe(facility, _recipe);
            }
        }
    }
}
