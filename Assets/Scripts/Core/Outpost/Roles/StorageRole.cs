using System;

namespace Scrapwalker.Core.Outpost.Roles
{
    [Serializable]
    public sealed class StorageRole : FacilityRole
    {
        public override bool HoldsMaterials => true;
    }
}
