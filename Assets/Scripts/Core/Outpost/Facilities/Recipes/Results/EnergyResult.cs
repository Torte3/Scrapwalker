using System;
using Scrapwalker.Core.Root;

namespace Scrapwalker.Core.Outpost.Facilities.Recipes.Results
{
    [Serializable]
    public sealed class EnergyResult : RecipeResult
    {
        public int Coal = 1;

        internal override bool UsesUpInputs => true;

        public float Amount(GameConfiguration configuration)
        {
            return configuration.CoalEnergy * Coal;
        }

        public override void Check(FacilityDefinition facility, RecipeDefinition recipe, IRecipeCheck check)
        {
            if (Coal < 1)
            {
                check.BadRecipe();
            }
        }

        public override void Show(IRecipeResultView view)
        {
            view.Generate(this);
        }

        internal override TaskStatus? OutputBlock(World world, Facility facility)
        {
            return world.Energy.Stored < world.Energy.Capacity ? (TaskStatus?)null : TaskStatus.NoEnergyRoom;
        }

        internal override bool Tick(World world, Facility facility, WorkJob job)
        {
            if (!job.Time.TryFillTick(facility.StaffSpeed))
            {
                return false;
            }

            bool done = job.Time.CountDownOne();
            world.Energy.Add(Amount(world.Configuration) / Math.Max(1, job.Time.Total));
            return done;
        }

        internal override void Finish(World world, Facility facility, WorkJob job)
        {
        }
    }
}
