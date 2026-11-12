using System;

namespace Scrapwalker.Core.Outpost.Roles
{
    [Serializable]
    public sealed class HandOverRole : FacilityRole
    {
        public float Seconds;

        public override float HandOverSeconds => Seconds;
    }
}
