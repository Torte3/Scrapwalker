using Scrapwalker.Core.Items;
using Scrapwalker.Core.Root;

namespace Scrapwalker.Core.Outpost.Facilities.Commands
{
    public sealed class AddWorkTask : FacilityCommand
    {
        public string RecipeID { get; }
        public int Times { get; }
        public bool Repeat { get; }
        public QualityFilter Filter { get; }

        public AddWorkTask(
            int facilityID, string recipeID, int times = 1, bool repeat = false, QualityFilter? filter = null)
            : base(facilityID)
        {
            RecipeID = recipeID;
            Times = times;
            Repeat = repeat;
            Filter = filter ?? QualityFilter.Any;
        }

        internal override bool IsAllowedBy(CommandRule rule, World world) => rule.AllowsTask(world, this);

        internal override string LogText => base.LogText + " " + RecipeID + "×" + Times + (Repeat ? " 繰り返し" : string.Empty) + " 投入 " + Filter.LogText;

        protected override CommandResult Check(World world, Facility facility)
        {
            CommandResult taskable = CommandResult.From(facility.Condition.Refusal(FacilityOrder.AddTask));
            if (!taskable.IsOK)
            {
                return taskable;
            }

            RecipeDefinition recipe = facility.Definition.FindRecipe(RecipeID);
            if (recipe == null)
            {
                return CommandResult.Fail(CommandError.NoSuchRecipe);
            }

            CommandResult offer = recipe.Offer(world, facility, Times).Result;
            if (!offer.IsOK)
            {
                return offer;
            }

            if (Times < 1)
            {
                return CommandResult.Fail(CommandError.BadCount);
            }

            if (!Filter.IsValid)
            {
                return CommandResult.Fail(CommandError.BadRange);
            }

            if (recipe.IsOneOff && (Times > 1 || Repeat))
            {
                return CommandResult.Fail(CommandError.OnceOnly);
            }

            if (recipe.IsOneOff && facility.Work.Queue.Contains(recipe))
            {
                return CommandResult.Fail(CommandError.AlreadyQueued);
            }

            return CommandResult.Success();
        }

        protected override void Run(World world, Facility facility)
        {
            RecipeDefinition recipe = facility.Definition.FindRecipe(RecipeID);
            for (int i = 0; i < Times; i++)
            {
                WorkTask task = facility.Work.Queue.Add(recipe, Repeat);
                task.Filter = Filter;
            }
        }
    }
}
