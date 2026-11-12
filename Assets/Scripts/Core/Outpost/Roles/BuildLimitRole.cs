using System;

namespace Scrapwalker.Core.Outpost.Roles
{
    [Serializable]
    public sealed class BuildLimitRole : FacilityRole
    {
        public int Count;

        public override int? BuildLimit => Count;
    }
}
