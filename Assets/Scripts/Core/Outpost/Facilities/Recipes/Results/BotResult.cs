using System;
using System.Linq;
using Scrapwalker.Core.Outpost.Bots;
using Scrapwalker.Core.Root;

namespace Scrapwalker.Core.Outpost.Facilities.Recipes.Results
{
    [Serializable]
    public sealed class BotResult : RecipeResult
    {
        public override int BotsMade => 1;

        public override void Describe(World world, Facility facility, RecipeDefinition recipe, IRecipeEffects effects)
        {
            effects.BotMade(world.Bots.Count, BotLimit(world));
        }

        internal override CommandResult CheckOffer(World world, Facility facility, RecipeDefinition recipe, int times)
        {
            if (BotsWithQueued(world) + times > BotLimit(world))
            {
                return CommandResult.Fail(CommandError.BotLimit);
            }

            return base.CheckOffer(world, facility, recipe, times);
        }

        internal override TaskStatus? ReadyBlock(World world, Facility facility, WorkTask task)
        {
            bool isFull = task != facility.Work.WorkingTask && BotsWithUnderway(world) >= BotLimit(world);
            return isFull ? TaskStatus.BotLimit : (TaskStatus?)null;
        }

        internal override void Finish(World world, Facility facility, WorkJob job)
        {
            Bot bot = world.AddBot(facility.Door);
            world.Log.Write(() => facility.LogName + " ボット#" + bot.ID + " ができた");
        }

        private static int BotLimit(World world)
        {
            return world.Upgrades.Traits.BotLimit;
        }

        private static int BotsWithQueued(World world)
        {
            return world.Bots.Count
                + world.Facilities.Sum(facility => facility.Work.Queue.Tasks.Sum(BotsMadeBy));
        }

        private static int BotsMadeBy(WorkTask task)
        {
            return task.Recipe.Result.BotsMade;
        }

        private static int BotsWithUnderway(World world)
        {
            return world.Bots.Count
                + world.Facilities.Sum(facility => facility.Work.WorkingTask?.Recipe.Result.BotsMade ?? 0);
        }
    }
}
