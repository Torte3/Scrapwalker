using System;

namespace Scrapwalker.Core.Combat.Weapons
{
    [Serializable]
    public sealed class ShotDefinition
    {
        public int Damage;
        public int DamagePerUpgrade;
        public float Radius;
        public float Depth;
        public int MaxTargets;
        public float ArmorPiercing;
        public float Knockback;
        public int Rounds = 1;
        public float RoundSeconds;
        public SlowEffect Slow = new SlowEffect();

        public bool Strikes => Damage > 0;

        public float NearReach => Radius;
        public float FarReach => Radius + Depth;

        public bool Covers(float landing, float distance)
        {
            float offset = distance - landing;
            return offset >= -NearReach && offset <= FarReach;
        }
    }
}
