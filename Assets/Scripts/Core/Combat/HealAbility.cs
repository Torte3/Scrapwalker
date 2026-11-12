using System;

namespace Scrapwalker.Core.Combat
{
    [Serializable]
    public sealed class HealAbility : EnemyAbility
    {
        public float Amount;
        public float Seconds;
        public float Reach;

        internal override EnemyTrait Start()
        {
            return new EnemyHealing(this);
        }
    }
}
