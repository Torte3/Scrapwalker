using Scrapwalker.Core.Outpost.Bots.Actions;
using Scrapwalker.Core.Outpost.Roles;
using Scrapwalker.Core.Root;

namespace Scrapwalker.Core.Outpost.Bots
{
    public static class BotEnergy
    {
        internal static void Drain(World world, Bot bot)
        {
            bot.BatterySeconds = System.Math.Max(0f, bot.BatterySeconds - world.Configuration.TickSeconds);
        }

        internal static bool SeekCharge(World world, Bot bot)
        {
            if (!bot.Action.GivesWayToCharging || !bot.NeedsCharge(world.Configuration) || !world.Energy.CanSupply)
            {
                return false;
            }

            Facility charger = FreeCharger(world, bot);
            if (charger == null || !world.Paths.CanReach(bot.Position, charger.Door))
            {
                return false;
            }

            if (!bot.IsIdle)
            {
                string dropped = bot.IsCarrying ? "（運んでいる資材はその場に置く）" : string.Empty;
                world.Log.Write(() => bot.LogName + " はバッテリー残量が減ったので " + bot.Action.LogText(world) + " を中断する" + dropped);
            }

            bot.Leave(world);
            bot.TryStart(world, new ChargeAction(charger));

            return true;
        }

        private static Facility FreeCharger(World world, Bot bot)
        {
            return world.Paths.Nearest(
                world.Facilities, facility => HasFreeSlot(world, facility), bot.Position, facility => facility.Door);
        }

        private static bool HasFreeSlot(World world, Facility charger)
        {
            return charger.IsOperational && world.CountCharging(charger) < (int)charger.Stat(FacilityStat.ChargerSlots);
        }
    }
}
