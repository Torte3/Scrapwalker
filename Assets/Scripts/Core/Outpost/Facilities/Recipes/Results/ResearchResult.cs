using System;
using Scrapwalker.Core.Framework;
using Scrapwalker.Core.Research;
using Scrapwalker.Core.Root;

namespace Scrapwalker.Core.Outpost.Facilities.Recipes.Results
{
    [Serializable]
    public sealed class ResearchResult : RecipeResult
    {
        public DefinitionReference<ResearchDefinition> ResearchReference =
            new DefinitionReference<ResearchDefinition>();

        public ResearchDefinition Research => ResearchReference.Definition;

        public override bool Completes(Definition goal)
        {
            return goal != null && goal == Research;
        }

        public override T ShowShelf<T>(IRecipeShelfView<T> view)
        {
            return view.Research();
        }

        public override bool IsSpent(World world, Facility facility, RecipeDefinition recipe)
        {
            return world.Research.IsDone(Research);
        }

        internal override CommandResult CheckOffer(World world, Facility facility, RecipeDefinition recipe, int times)
        {
            if (world.Research.IsDone(Research))
            {
                return CommandResult.Fail(CommandError.ResearchDone);
            }

            if (world.Research.Current != null)
            {
                return CommandResult.Fail(CommandError.ResearchBusy);
            }

            return world.Research.ArePrerequisitesDone(Research) ? CommandResult.Success() : CommandResult.Fail(CommandError.ResearchLocked);
        }

        public override void Check(FacilityDefinition facility, RecipeDefinition recipe, IRecipeCheck check)
        {
            if (Research == null)
            {
                check.MissingResearch();
            }
        }

        internal override void Finish(World world, Facility facility, WorkJob job)
        {
            world.Emit(new ResearchWorkFinished(Research));
        }
    }
}
