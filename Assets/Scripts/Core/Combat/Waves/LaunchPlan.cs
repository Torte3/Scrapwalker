using System;
using Scrapwalker.Core.Outpost.Facilities.Recipes;
using Scrapwalker.Core.Root;
using Scrapwalker.Core.Travel;

namespace Scrapwalker.Core.Combat.Waves
{
    [Serializable]
    public sealed class LaunchPlan : RuinVisitPlan
    {
        public float LandToWaveSeconds;
        public float WarningSeconds;

        public override bool IsDigging => false;

        public override bool CanBeCleared => false;

        public override bool TakesOffAfter(RuinDefinition ruin, int number) => number == ruin.WaveCount;

        public override bool IsCharging(World world) => world.InCombat && world.Visit.Phase.IsUnderway;

        public override int ChargeTicks(GameConfiguration configuration, RuinDefinition ruin)
        {
            WaveDefinition last = ruin.WaveAt(ruin.WaveCount);
            return configuration.TicksFor(LandToWaveSeconds) + configuration.TicksFor(last != null ? last.RetreatSeconds : 0f);
        }

        public override void DescribeOpening(IRecipeEffects effects)
        {
            effects.Note(EffectNote.EscapeSequence);
        }

        internal override void BeginAfterLanding(World world)
        {
            world.Visit.Enter(WavePhase.Resting, world.Configuration.TicksFor(LandToWaveSeconds));
        }

        internal override void BeginReach(World world, float seconds)
        {
            RuinVisitSystem.StartWave(world, world.Wave.Number + 1);
        }

        internal override void Rest(World world, RuinDefinition ruin)
        {
            if (world.Visit.PhaseTime.Remaining == world.Configuration.TicksFor(WarningSeconds))
            {
                world.Emit(new WaveNearing(ruin, world.Wave.Number + 1));
            }
        }

        internal override void ClearWave(World world, RuinDefinition ruin, int number)
        {
            RuinVisitSystem.FinishWave(world, ruin);
        }

        internal override void FinishLastWave(World world, RuinDefinition ruin)
        {
            bool defeatedAll = !world.Wave.TimedOut;
            RuinVisitSystem.Reset(world);
            world.Emit(new TookOff(ruin, defeatedAll));
        }

        internal override bool LeavesAfter(World world, RuinDefinition ruin) => false;

        internal override float RestSeconds(GameConfiguration configuration)
        {
            return Math.Max(0f, configuration.WaveGapSeconds);
        }

        internal override void OnChosen(World world)
        {
            world.Emit(new EscapeChosen());
        }
    }
}
