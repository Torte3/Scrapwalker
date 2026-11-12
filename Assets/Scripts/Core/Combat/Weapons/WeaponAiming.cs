using Scrapwalker.Core.Outpost;
using Scrapwalker.Core.Root;

namespace Scrapwalker.Core.Combat.Weapons
{
    internal static class WeaponAiming
    {
        internal static int FlightTicks(World world, Facility gun, Enemy target)
        {
            float deltaTime = world.Configuration.TickSeconds;
            int low = 1;
            int high = WeaponSystem.FlightTicks(world, gun, target.Distance);
            while (low < high)
            {
                int middle = (low + high) / 2;
                if (WeaponSystem.FlightTicks(world, gun, target.DistanceAfter(middle, deltaTime)) <= middle)
                {
                    high = middle;
                }
                else
                {
                    low = middle + 1;
                }
            }

            return low;
        }

        internal static float LandingDistance(World world, Enemy target, int ticks)
        {
            return target.DistanceAfter(ticks, world.Configuration.TickSeconds);
        }
    }
}
