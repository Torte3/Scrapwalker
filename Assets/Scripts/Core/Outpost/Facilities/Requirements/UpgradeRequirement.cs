using System;
using Scrapwalker.Core.Outpost.Facilities.Recipes;
using Scrapwalker.Core.Root;

namespace Scrapwalker.Core.Outpost.Facilities.Requirements
{
    [Serializable]
    public sealed class UpgradeRequirement : Requirement
    {
        public string Upgrade = string.Empty;

        internal override CommandResult Refusal => CommandResult.Fail(CommandError.NeedsUpgrade);

        public override bool IsMet(World world, Facility facility)
        {
            return facility.Work.Upgrades.Has(Upgrade);
        }

        public override bool IsOpenedBy(RecipeDefinition upgrade)
        {
            return upgrade.ID == Upgrade;
        }

        public override bool CanBeMet(FacilityDefinition facility, IRequirementSupply supply)
        {
            return supply.CanRun(facility.FindRecipe(Upgrade));
        }

        public override void Check(FacilityDefinition facility, IRecipeCheck check)
        {
            if (facility.FindRecipe(Upgrade) == null)
            {
                check.MissingRecipe(Upgrade);
            }
        }
    }
}
