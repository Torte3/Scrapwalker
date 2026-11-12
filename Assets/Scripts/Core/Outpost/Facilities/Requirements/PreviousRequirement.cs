using System;
using Scrapwalker.Core.Outpost.Facilities.Recipes;
using Scrapwalker.Core.Root;

namespace Scrapwalker.Core.Outpost.Facilities.Requirements
{
    [Serializable]
    public sealed class PreviousRequirement : Requirement
    {
        public string Recipe = string.Empty;

        internal override bool HidesUntilMet => false;

        internal override CommandResult Refusal => CommandResult.Fail(CommandError.PreviousNotDone, Recipe);

        public override bool IsMet(World world, Facility facility)
        {
            RecipeDefinition previous = facility.Definition.FindRecipe(Recipe);
            return previous == null || previous.Result.IsSpent(world, facility, previous);
        }

        internal override RecipeDefinition PreviousIn(FacilityDefinition facility)
        {
            return facility.FindRecipe(Recipe);
        }

        public override void Check(FacilityDefinition facility, IRecipeCheck check)
        {
            if (facility.FindRecipe(Recipe) == null)
            {
                check.MissingRecipe(Recipe);
            }
        }
    }
}
