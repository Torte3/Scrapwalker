using Scrapwalker.Core.Framework;
using Scrapwalker.Core.Outpost;

namespace Scrapwalker.Core.Combat.Weapons
{
    public sealed class Projectile
    {
        public int ID { get; internal set; }
        public Facility Gun { get; }
        public ShotDefinition Shot { get; }
        public float Origin { get; }
        public float LandingDistance { get; }
        public float Damage { get; }
        public Countdown Time { get; } = new Countdown();

        public Projectile(Facility gun, ShotDefinition shot, float origin, float landingDistance, float damage, int ticks)
        {
            Gun = gun;
            Shot = shot;
            Origin = origin;
            LandingDistance = landingDistance;
            Damage = damage;
            Time.Start(ticks);
        }
    }
}
