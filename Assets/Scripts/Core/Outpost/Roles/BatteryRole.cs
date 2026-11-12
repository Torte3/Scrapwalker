using System;
using Scrapwalker.Core.Root;

namespace Scrapwalker.Core.Outpost.Roles
{
    [Serializable]
    public sealed class BatteryRole : FacilityRole
    {
        public override float Fill(World world, Facility facility)
        {
            return world.Energy.Fill;
        }
    }
}
