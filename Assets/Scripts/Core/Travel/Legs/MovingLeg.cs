using Scrapwalker.Core.Root;

namespace Scrapwalker.Core.Travel.Legs
{
    public sealed class MovingLeg : JourneyLeg
    {
        internal override void Tick(World world, Journey journey)
        {
            if (journey.Destination != null)
            {
                journey.BeginArrival(world.Configuration.TicksFor(world.Configuration.TravelSeconds));
            }
        }
    }
}
