using System;
using Scrapwalker.Core.Root;

namespace Scrapwalker.Core.Outpost.Roles
{
    [Serializable]
    public sealed class BarrierRole : FacilityRole
    {
        public override float Fill(World world, Facility facility)
        {
            return world.Barrier.Fill;
        }

        public override bool IsRecovering(World world)
        {
            return world.Barrier.IsRecovering;
        }
    }
}
