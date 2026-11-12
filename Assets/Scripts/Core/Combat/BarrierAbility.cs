using System;

namespace Scrapwalker.Core.Combat
{
    [Serializable]
    public sealed class BarrierAbility : EnemyAbility
    {
        public int Layers;
        public float RecoverSeconds;

        internal override EnemyTrait Start()
        {
            return new EnemyBarrier(this);
        }
    }
}
