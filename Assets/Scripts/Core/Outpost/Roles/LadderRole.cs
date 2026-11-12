using System;

namespace Scrapwalker.Core.Outpost.Roles
{
    [Serializable]
    public sealed class LadderRole : FacilityRole
    {
        public override bool ConnectsFloors => true;
    }
}
