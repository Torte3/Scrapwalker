using System;
using Scrapwalker.Core.Framework;

namespace Scrapwalker.Core.Combat
{
    [Serializable]
    public struct AttackMove
    {
        public DefinitionReference<AttackStyleDefinition> StyleReference;
        public int Shots;
        public bool HasOneTarget;
        public int Damage;
        public float ImpactSeconds;
        public float GapSeconds;
        public float TailSeconds;
        public float RestSeconds;
        public float Weight;

        public AttackStyleDefinition Style => StyleReference?.Definition;
        public int ShotCount => Math.Max(1, Shots);
        public float Duration => ImpactAt(ShotCount - 1) + TailSeconds;

        public float ImpactAt(int shot)
        {
            return ImpactSeconds + shot * GapSeconds;
        }
    }
}
