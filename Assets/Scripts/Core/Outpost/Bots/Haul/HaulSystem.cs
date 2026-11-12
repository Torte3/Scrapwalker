using Scrapwalker.Core.Outpost.Bots.Actions;
using Scrapwalker.Core.Outpost.Floors;
using Scrapwalker.Core.Root;

namespace Scrapwalker.Core.Outpost.Bots.Haul
{
    internal static class HaulSystem
    {
        internal static void Reroute(World world, Facility facility)
        {
            foreach (Bot bot in world.Bots)
            {
                bot.Action.OnFacilityMoved(world, bot, facility);
            }
        }

        internal static void DropOnFloor(World world, Bot bot)
        {
            if (bot.IsCarrying)
            {
                SpillSystem.Drop(world, bot.Position, bot.Cargo);
            }

            bot.ClearCargo();
        }

        internal static void PutAway(World world, Bot bot)
        {
            if (!bot.IsCarrying)
            {
                bot.GoIdle();
                return;
            }

            Facility store = StoreWithFreeSpace(world, bot);
            if (store != null && bot.TryHeadTo(world, new DropoffAction(store, bot), store.Door))
            {
                return;
            }

            DropOnFloor(world, bot);
            bot.GoIdle();
        }

        internal static bool GoToDestination(World world, Bot bot, HaulAction haul)
        {
            DeliveryTarget target = haul.Destination.Resolve(world)?.DeliveryFor(world, bot.Position, bot.Cargo.Count);
            if (target != null && bot.TryHeadTo(world, haul, target.Door))
            {
                haul.Redirect(target.Place);
                return true;
            }

            Facility store = StoreWithFreeSpace(world, bot);
            if (store == null || !bot.TryHeadTo(world, haul, store.Door))
            {
                return false;
            }

            haul.Redirect(Place.InStore(store));
            return true;
        }

        private static Facility StoreWithFreeSpace(World world, Bot bot)
        {
            return world.Stock.StoreWithFreeSpace(bot.Position, bot.Cargo.Count);
        }
    }
}
