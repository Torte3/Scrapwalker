using Scrapwalker.Core.Items;
using Scrapwalker.Core.Outpost;

namespace Scrapwalker.Core.Combat.Weapons
{
    internal sealed class FireRequest
    {
        public Facility Gun { get; }
        public ShotDefinition Shot { get; }
        public Quality BulletQuality { get; }
        public Enemy Target { get; }

        internal FireRequest(Facility gun, ShotDefinition shot, Quality bulletQuality, Enemy target)
        {
            Gun = gun;
            Shot = shot;
            BulletQuality = bulletQuality;
            Target = target;
        }
    }
}
