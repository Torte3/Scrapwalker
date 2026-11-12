using System;

namespace Scrapwalker.Core.Combat.Weapons
{
    [Serializable]
    public sealed class SlowEffect
    {
        public float Factor = 1f;
        public float Seconds;

        public bool IsActive => Seconds > 0f;
    }
}
