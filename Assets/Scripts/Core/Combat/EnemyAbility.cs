using System;

namespace Scrapwalker.Core.Combat
{
    [Serializable]
    public abstract class EnemyAbility
    {
        internal abstract EnemyTrait Start();
    }
}
