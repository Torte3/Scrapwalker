using System.Linq;
using Scrapwalker.Core.Root;
using Scrapwalker.Core.Travel;

namespace Scrapwalker.Core.Combat.Waves
{
    internal static class RuinVisitSystem
    {
        internal static void Listen(World world)
        {
            world.Events.On<Landed>(landed =>
            {
                world.Battle.Clear();
                Reset(world);
                landed.Ruin.Plan.BeginAfterLanding(world);
            });
            world.Events.On<Departed>(_ =>
            {
                world.Battle.Clear();
                Reset(world);
            });
        }

        internal static void Reset(World world)
        {
            world.Wave.Reset();
            world.Visit.Reset();
        }

        internal static void Tick(World world)
        {
            if (!world.InCombat)
            {
                return;
            }

            world.Visit.Tick(world, world.Journey.CurrentRuin);
        }

        internal static void StartWave(World world, int number)
        {
            RuinDefinition ruin = world.Journey.CurrentRuin;
            WaveDefinition wave = ruin.WaveAt(number);
            world.Wave.StartFighting(number, ruin.SpawnOrder(number).Select(s => new QueuedEnemy(s.Enemy, world.Configuration.TicksFor(s.At))), wave != null ? wave.RetreatSeconds : 0f, world.Configuration);
            world.Visit.Enter(WavePhase.Fighting, 0);
            world.Emit(new WaveStarted(ruin, number));
        }

        internal static void ClearWave(World world, RuinDefinition ruin)
        {
            int number = world.Wave.Number;
            world.Emit(new WaveCleared(ruin, number, ruin.Plan.LeavesAfter(world, ruin)));
            ruin.Plan.ClearWave(world, ruin, number);
        }

        internal static void FinishWave(World world, RuinDefinition ruin)
        {
            if (world.Wave.Number < ruin.WaveCount)
            {
                world.Visit.Enter(WavePhase.Resting, world.Configuration.TicksFor(ruin.Plan.RestSeconds(world.Configuration)));
                return;
            }

            ruin.Plan.FinishLastWave(world, ruin);
        }
    }
}
