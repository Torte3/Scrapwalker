using System;
using Scrapwalker.Core.Outpost.Bots;
using Scrapwalker.Core.Root;

namespace Scrapwalker.Core.Outpost.Roles
{
    [Serializable]
    public sealed class ChargerRole : FacilityRole
    {
        public override float Fill(World world, Facility facility)
        {
            Bot charging = ChargingBot(world, facility);
            return charging != null ? charging.BatteryFill(world.Configuration) : 0f;
        }

        public override bool IsInUse(World world, Facility facility)
        {
            return ChargingBot(world, facility) != null;
        }

        private static Bot ChargingBot(World world, Facility facility)
        {
            return facility.IsOperational ? world.BotCharging(facility) : null;
        }
    }
}
