using Scrapwalker.Core.Outpost;
using Scrapwalker.Core.Outpost.Facilities;
using Scrapwalker.Core.Outpost.Facilities.Commands;
using Scrapwalker.Core.Root;

namespace Scrapwalker.Core.Sessions.Tutorial.Goals
{
    public abstract class FacilityGoal : TutorialGoal
    {
        private readonly NamedDefinition<FacilityDefinition> _facility;

        protected FacilityGoal(string facility)
        {
            _facility = new NamedDefinition<FacilityDefinition>(facility);
        }

        public FacilityDefinition FacilityOf(World world)
        {
            return _facility.Of(world);
        }

        public sealed override void Check(GameDefinitions definitions, ITutorialGoalCheck check)
        {
            if (!_facility.TryIn(definitions, out FacilityDefinition definition))
            {
                check.MissingFacility(_facility.Key);
                return;
            }

            CheckIn(definition, definitions, check);
        }

        protected virtual void CheckIn(FacilityDefinition facility, GameDefinitions definitions, ITutorialGoalCheck check)
        {
        }

        protected RecipeDefinition RecipeOf(World world, AddWorkTask task)
        {
            Facility f = world.Facilities.Find(task.FacilityID);
            return f != null && f.Definition == FacilityOf(world) ? f.Definition.FindRecipe(task.RecipeID) : null;
        }
    }
}
