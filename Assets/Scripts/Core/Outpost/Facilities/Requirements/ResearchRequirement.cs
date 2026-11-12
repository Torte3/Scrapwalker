using System;
using Scrapwalker.Core.Framework;
using Scrapwalker.Core.Outpost.Facilities.Recipes;
using Scrapwalker.Core.Research;
using Scrapwalker.Core.Root;

namespace Scrapwalker.Core.Outpost.Facilities.Requirements
{
    [Serializable]
    public sealed class ResearchRequirement : Requirement
    {
        public DefinitionReference<ResearchDefinition> ResearchReference = new DefinitionReference<ResearchDefinition>();

        public ResearchRequirement()
        {
        }

        public ResearchRequirement(ResearchDefinition research)
        {
            ResearchReference = new DefinitionReference<ResearchDefinition>(research);
        }

        public override ResearchDefinition Research => ResearchReference.Definition;

        internal override CommandResult Refusal => CommandResult.Fail(CommandError.Locked);

        public override bool IsMet(World world, Facility facility)
        {
            return world.Research.IsDone(Research);
        }

        public override ResearchDefinition PendingResearch(World world)
        {
            return world.Research.IsDone(Research) ? null : Research;
        }

        public override bool CanBeMet(FacilityDefinition facility, IRequirementSupply supply)
        {
            return supply.IsDone(Research);
        }

        public override void Check(FacilityDefinition facility, IRecipeCheck check)
        {
            if (Research == null)
            {
                check.MissingResearch();
            }
        }
    }
}
