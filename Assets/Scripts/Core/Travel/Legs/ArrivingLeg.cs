using Scrapwalker.Core.Framework;
using Scrapwalker.Core.Root;

namespace Scrapwalker.Core.Travel.Legs
{
    public sealed class ArrivingLeg : JourneyLeg
    {
        private readonly Countdown _time = new Countdown();

        internal ArrivingLeg(int ticks)
        {
            _time.Start(ticks);
        }

        public override Countdown Route => _time;

        public override RuinDefinition RuinAhead(Journey journey)
        {
            return journey.Destination;
        }

        internal override JourneyLeg WithoutDestination => new MovingLeg();

        internal override void Tick(World world, Journey journey)
        {
            if (_time.CountDownOne())
            {
                JourneySystem.Arrive(world, journey.Destination);
            }
        }
    }
}
