using Scrapwalker.Core.Root;
using Scrapwalker.Core.Travel;

namespace Scrapwalker.Core.Combat.Waves
{
    public abstract class WavePhase
    {
        public static readonly WavePhase Idle = new IdlePhase();
        public static readonly WavePhase Reaching = new ReachingPhase();
        public static readonly WavePhase Fighting = new FightingPhase();
        public static readonly WavePhase Haul = new HaulPhase();
        public static readonly WavePhase Resting = new RestingPhase();

        private WavePhase()
        {
        }

        public virtual bool IsUnderway => true;
        public virtual bool IsFighting => false;

        public abstract T Show<T>(IWavePhaseView<T> view);

        internal virtual float ArmProgress(GameConfiguration configuration, float remainingSeconds)
        {
            return 0f;
        }

        internal abstract void Tick(World world, RuinVisit visit, RuinDefinition ruin);

        private sealed class IdlePhase : WavePhase
        {
            public override bool IsUnderway => false;

            public override T Show<T>(IWavePhaseView<T> view)
            {
                return view.Idle();
            }

            internal override void Tick(World world, RuinVisit visit, RuinDefinition ruin)
            {
            }
        }

        private sealed class ReachingPhase : WavePhase
        {
            public override T Show<T>(IWavePhaseView<T> view)
            {
                return view.Reaching();
            }

            internal override float ArmProgress(GameConfiguration configuration, float remainingSeconds)
            {
                return 1f - remainingSeconds / configuration.ArmReachSeconds;
            }

            internal override void Tick(World world, RuinVisit visit, RuinDefinition ruin)
            {
                if (visit.PhaseTime.CountDownOne())
                {
                    RuinVisitSystem.StartWave(world, world.Wave.Number + 1);
                }
            }
        }

        private sealed class FightingPhase : WavePhase
        {
            public override bool IsFighting => true;

            public override T Show<T>(IWavePhaseView<T> view)
            {
                return view.Fighting();
            }

            internal override void Tick(World world, RuinVisit visit, RuinDefinition ruin)
            {
                if (WaveSystem.Tick(world, definition => SalvageSystem.DrawDrops(world, definition)))
                {
                    RuinVisitSystem.ClearWave(world, ruin);
                }
            }
        }

        private sealed class HaulPhase : WavePhase
        {
            public override T Show<T>(IWavePhaseView<T> view)
            {
                return view.Haul();
            }

            internal override float ArmProgress(GameConfiguration configuration, float remainingSeconds)
            {
                return 1f - remainingSeconds / configuration.HaulSeconds;
            }

            internal override void Tick(World world, RuinVisit visit, RuinDefinition ruin)
            {
                if (visit.PhaseTime.CountDownOne())
                {
                    RuinVisitSystem.FinishWave(world, ruin);
                }
            }
        }

        private sealed class RestingPhase : WavePhase
        {
            public override T Show<T>(IWavePhaseView<T> view)
            {
                return view.Resting();
            }

            internal override void Tick(World world, RuinVisit visit, RuinDefinition ruin)
            {
                if (visit.PhaseTime.CountDownOne())
                {
                    ruin.Plan.BeginReach(world, world.Configuration.ArmReachSeconds);
                }
                else
                {
                    ruin.Plan.Rest(world, ruin);
                }
            }
        }
    }
}
