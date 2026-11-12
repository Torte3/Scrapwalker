using System;
using Scrapwalker.Core.Outpost.Facilities.Recipes;
using Scrapwalker.Core.Root;
using Scrapwalker.Core.Travel;

namespace Scrapwalker.Core.Combat.Waves
{
    [Serializable]
    public sealed class SalvagePlan : RuinVisitPlan
    {
        public override bool IsDigging => true;

        public override bool CanBeCleared => true;

        public override void DescribeOpening(IRecipeEffects effects)
        {
            effects.Note(EffectNote.RuinsFound);
        }

        internal override void BeginAfterLanding(World world)
        {
            BeginReach(world, world.Configuration.LandToDigSeconds);
        }

        internal override void BeginReach(World world, float seconds)
        {
            world.Visit.Enter(WavePhase.Reaching, world.Configuration.TicksFor(seconds));
        }

        internal override void ClearWave(World world, RuinDefinition ruin, int number)
        {
            world.Visit.Enter(WavePhase.Haul, world.Configuration.TicksFor(world.Configuration.HaulSeconds));
            SalvageSystem.CollectWaveLoot(world, ruin, number);
        }

        internal override void FinishLastWave(World world, RuinDefinition ruin)
        {
            bool leaves = LeavesAfter(world, ruin);
            world.Emit(new RuinCompleted(ruin));
            if (!leaves)
            {
                RuinVisitSystem.Reset(world);
                BeginReach(world, world.Configuration.ArmReachSeconds);
            }
        }

        internal override bool LeavesAfter(World world, RuinDefinition ruin)
        {
            return world.Wave.Number >= ruin.WaveCount && !world.Journey.IsRepeating;
        }

        internal override float RestSeconds(GameConfiguration configuration)
        {
            return Math.Max(0f, configuration.WaveGapSeconds - (configuration.HaulSeconds + configuration.ArmReachSeconds));
        }
    }
}
