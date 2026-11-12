using Scrapwalker.Core.Items;
using Scrapwalker.Core.Outpost;
using Scrapwalker.Core.Outpost.Grid;
using Scrapwalker.Core.Root;
using Scrapwalker.Core.Sessions.Tutorial;

namespace Scrapwalker.Core.Sessions
{
    public static class WorldFactory
    {
        private const float FirstBotFromIntake = 1.5f;
        private const float BotSpacing = 0.5f;

        public static World CreateTutorial(GameDefinitions definitions, TutorialConfiguration tutorial)
        {
            World world = CreateBase(definitions);
            foreach (StartFacility facility in tutorial.Wrecks)
            {
                FacilityCondition.Wreck(world, world.AddFacility(facility.Facility, facility.Cell));
            }

            return world;
        }

        private static World CreateBase(GameDefinitions definitions)
        {
            var world = new World(definitions);
            GameConfiguration configuration = world.Configuration;
            foreach (StartFacility facility in configuration.StartFacilities)
            {
                Facility f = world.AddFacility(facility.Facility, facility.Cell);
                f.Site.Delivered.AddRange(StackList.Of(f.Definition.Cost, configuration.StartQuality).Stacks);
                f.Condition.Complete(world, f);
            }

            world.Energy.Stored = world.Energy.Capacity;

            int bots = configuration.StartBots;
            for (int i = 0; i < bots; i++)
            {
                world.AddBot(new GridPoint(world.Intake.X - FirstBotFromIntake - i * BotSpacing, 0f));
            }

            return world;
        }

        internal static void AddStartMaterials(World world)
        {
            GameConfiguration configuration = world.Configuration;
            world.Progress.NoteObtained(configuration.StartQuality);
            foreach (MaterialAmount amount in configuration.StartMaterials)
            {
                world.Stock.StoreOrDrop(ItemStack.Of(amount.Type, configuration.StartQuality, amount.Count));
            }
        }
    }
}
