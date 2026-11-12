using Scrapwalker.Core.Outpost;
using Scrapwalker.Core.Root;
using Scrapwalker.Core.Travel;

namespace Scrapwalker.Core.Sessions
{
    internal static class DefeatSystem
    {
        internal static void Listen(World world)
        {
            world.Events.On<HullBreached>(_ => Begin(world));
        }

        internal static void Begin(World world)
        {
            world.Log.Write(() => "外壁が破られた");
            world.DefeatBlackout.Start(world.Configuration.DefeatDarkenSeconds, world.Configuration.DefeatHoldSeconds, world.Configuration.DefeatBrightenSeconds);
            world.Emit(new Defeated());
        }

        internal static void Tick(World world)
        {
            world.DefeatBlackout.Advance(world.Configuration.TickSeconds, () => Restart(world));
            if (!world.DefeatBlackout.IsActive)
            {
                Recover(world);
            }
        }

        private static void Recover(World world)
        {
            bool isFirst = !world.Progress.HasBeenDefeated;
            if (isFirst)
            {
                world.Progress.HasBeenDefeated = true;
            }

            world.Emit(new DefeatBlackoutEnded(isFirst));
        }

        private static void Restart(World world)
        {
            world.Log.Write(() => "移動中から再開");
            JourneySystem.RestartAfterDefeat(world);
            world.Hull.RestoreAfterDefeat();
            world.Emit(new Restarted());
        }
    }
}
