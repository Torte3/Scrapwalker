using System;
using Scrapwalker.Core.Outpost.Facilities.Recipes;
using Scrapwalker.Core.Root;
using Scrapwalker.Core.Travel;

namespace Scrapwalker.Core.Combat.Waves
{
    [Serializable]
    public abstract class RuinVisitPlan
    {
        public abstract bool IsDigging { get; }

        public abstract bool CanBeCleared { get; }

        public virtual bool TakesOffAfter(RuinDefinition ruin, int number) => false;

        public virtual bool IsCharging(World world) => false;

        public virtual int ChargeTicks(GameConfiguration configuration, RuinDefinition ruin) => 0;

        public abstract void DescribeOpening(IRecipeEffects effects);

        internal abstract void BeginAfterLanding(World world);

        internal abstract void BeginReach(World world, float seconds);

        internal virtual void Rest(World world, RuinDefinition ruin)
        {
        }

        internal abstract void ClearWave(World world, RuinDefinition ruin, int number);

        internal abstract void FinishLastWave(World world, RuinDefinition ruin);

        internal abstract bool LeavesAfter(World world, RuinDefinition ruin);

        internal abstract float RestSeconds(GameConfiguration configuration);

        internal virtual void OnChosen(World world)
        {
        }
    }
}
