using Scrapwalker.Core.Root;

namespace Scrapwalker.Core.Outpost.Bots
{
    internal static class BotSystem
    {
        internal static void Tick(World world)
        {
            foreach (Facility facility in world.Facilities)
            {
                facility.IsStaffed = false;
            }

            world.Assigner.AssignStations(world);

            foreach (Bot bot in world.Bots)
            {
                Prepare(world, bot);
            }

            world.Assigner.Assign(world);
            foreach (Bot bot in world.Bots)
            {
                bool working = bot.Action.UsesBattery(world, bot);
                bot.Action.Tick(world, bot);
                if (working)
                {
                    BotEnergy.Drain(world, bot);
                }
            }
        }

        private static void Prepare(World world, Bot bot)
        {
            if (BotEnergy.SeekCharge(world, bot) || bot.Action.CanContinue(world))
            {
                return;
            }

            world.Log.Write(() => bot.LogName + " の " + bot.Action.LogText(world) + " は続けられない → 待機");

            bot.Leave(world);
        }
    }
}
