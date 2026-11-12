using System;
using System.Collections.Generic;
using System.Linq;
using Scrapwalker.Core.Framework;
using Scrapwalker.Core.Outpost.Upgrades;
using Scrapwalker.Core.Root;
using Scrapwalker.Core.Travel;

namespace Scrapwalker.Core.Outpost.Facilities.Recipes.Results
{
    [Serializable]
    public sealed class OutpostUpgradeResult : RecipeResult
    {
        public DefinitionReference<OutpostUpgradeDefinition> UpgradeReference = new DefinitionReference<OutpostUpgradeDefinition>();

        public OutpostUpgradeDefinition Upgrade
        {
            get => UpgradeReference?.Definition;
            set => UpgradeReference = new DefinitionReference<OutpostUpgradeDefinition>(value);
        }

        public override bool Completes(Definition goal)
        {
            return goal != null && goal == Upgrade;
        }

        public override bool IsSpent(World world, Facility facility, RecipeDefinition recipe)
        {
            return world.Upgrades.IsDone(Upgrade);
        }

        internal override CommandResult CheckOffer(World world, Facility facility, RecipeDefinition recipe, int times)
        {
            OutpostUpgradeDefinition missing = Upgrade.Prerequisites.FirstOrDefault(prerequisite => !world.Upgrades.IsDone(prerequisite));
            return missing == null ? CommandResult.Success() : CommandResult.Fail(CommandError.PreviousNotDone, world.Definitions.RecipeFor(missing)?.ID);
        }

        public override void Check(FacilityDefinition facility, RecipeDefinition recipe, IRecipeCheck check)
        {
            if (Upgrade == null)
            {
                check.BadRecipe();
            }
        }

        public override void Describe(World world, Facility facility, RecipeDefinition recipe, IRecipeEffects effects)
        {
            OutpostTraits before = world.Upgrades.TraitsBefore(Upgrade);
            foreach (UpgradeEffect effect in Upgrade.Effects)
            {
                effect.Describe(before, effects);
            }

            RuinDefinition opened = world.Definitions.Ruins.FirstOrDefault(ruin => ruin.Needs(Upgrade));
            opened?.Plan.DescribeOpening(effects);
        }

        internal override void Finish(World world, Facility facility, WorkJob job)
        {
            world.Upgrades.Add(Upgrade);
            world.Log.Write(() => Upgrade.Key + ": 「" + job.Task.Recipe.ID + "」が済んだ。ボットの上限 " + world.Upgrades.Traits.BotLimit);
            List<RuinDefinition> opened = world.Exploration.OpenedBy(world.Definitions.Ruins, Upgrade).ToList();
            world.Emit(new OutpostUpgraded(Upgrade, opened));
        }
    }
}
