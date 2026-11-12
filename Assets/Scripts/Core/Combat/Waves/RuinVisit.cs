using Scrapwalker.Core.Framework;
using Scrapwalker.Core.Root;
using Scrapwalker.Core.Travel;

namespace Scrapwalker.Core.Combat.Waves
{
    public sealed class RuinVisit
    {
        public WavePhase Phase { get; private set; } = WavePhase.Idle;
        public Countdown PhaseTime { get; } = new Countdown();
        public ElapsedTicks SinceLanding { get; } = new ElapsedTicks();

        public float ArmProgress(GameConfiguration configuration, float share)
        {
            float remainingSeconds = System.Math.Max(0f, PhaseTime.Remaining - share) * configuration.TickSeconds;
            return Phase.ArmProgress(configuration, remainingSeconds);
        }

        internal void Reset()
        {
            Enter(WavePhase.Idle, 0);
            SinceLanding.Reset();
        }

        internal void Tick(World world, RuinDefinition ruin)
        {
            SinceLanding.Tick();
            Phase.Tick(world, this, ruin);
        }

        internal void Enter(WavePhase phase, int ticks)
        {
            Phase = phase;
            PhaseTime.Start(ticks);
        }
    }
}
