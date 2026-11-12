using System;
using Scrapwalker.Core.Outpost.Facilities.Recipes;
using Scrapwalker.Core.Research;
using Scrapwalker.Core.Root;

namespace Scrapwalker.Core.Outpost.Facilities.Requirements
{
    [Serializable]
    public abstract class Requirement
    {
        public virtual ResearchDefinition Research => null;

        internal virtual bool HidesUntilMet => true;

        public abstract bool IsMet(World world, Facility facility);

        internal abstract CommandResult Refusal { get; }

        public virtual ResearchDefinition PendingResearch(World world)
        {
            return null;
        }

        public virtual bool IsOpenedBy(RecipeDefinition upgrade)
        {
            return false;
        }

        internal virtual RecipeDefinition PreviousIn(FacilityDefinition facility)
        {
            return null;
        }

        public virtual bool CanBeMet(FacilityDefinition facility, IRequirementSupply supply)
        {
            return true;
        }

        public virtual void Check(FacilityDefinition facility, IRecipeCheck check)
        {
        }
    }
}
