using Scrapwalker.Core.Outpost.Upgrades;
using System.Collections.Generic;
using Scrapwalker.Core.Root;

namespace Scrapwalker.Core.Travel
{
    internal static class JourneySystem
    {
        internal static void Listen(World world)
        {
            world.Events.On<RuinCompleted>(completed => Complete(world, completed.Ruin));
        }

        internal static void Tick(World world)
        {
            world.Journey.Leg.Tick(world, world.Journey);
        }

        private static int TicksToLeaveRuinBehind(GameConfiguration configuration)
        {
            return configuration.TicksFor(configuration.TravelClearSeconds - configuration.TravelSeconds);
        }

        internal static void Leave(World world)
        {
            Depart(world, false);
        }

        internal static void RestartAfterDefeat(World world)
        {
            Depart(world, true);
            world.Journey.MoveOn();
        }

        private static void Depart(World world, bool afterDefeat)
        {
            world.Journey.Depart(TicksToLeaveRuinBehind(world.Configuration));
            world.Emit(new Departed(world.Journey.RuinBehind, afterDefeat));
        }

        internal static void Arrive(World world, RuinDefinition ruin)
        {
            world.Emit(new Arriving(ruin));
            world.Log.Write(() => "接地: " + ruin.Key);
            world.Journey.Land(ruin);
            world.Exploration.NoteReached(ruin);
            world.Emit(new Landed(ruin));
        }

        private static void Complete(World world, RuinDefinition ruin)
        {
            Exploration exploration = world.Exploration;
            bool first = !exploration.HasCleared(ruin);
            exploration.MarkCleared(ruin);
            if (first && exploration.HasClearedAllOpen(world.Definitions.Ruins))
            {
                IReadOnlyList<OutpostUpgradeDefinition> awaited = exploration.UpgradesAwaited(world.Definitions.Ruins);
                if (awaited.Count > 0)
                {
                    world.Emit(new RuinsInSightCleared(awaited));
                }
            }

            if (!world.Journey.IsRepeating)
            {
                Leave(world);
            }
        }
    }
}
