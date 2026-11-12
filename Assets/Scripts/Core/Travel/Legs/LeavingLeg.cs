using Scrapwalker.Core.Framework;
using Scrapwalker.Core.Root;

namespace Scrapwalker.Core.Travel.Legs
{
    public sealed class LeavingLeg : JourneyLeg
    {
        private readonly RuinDefinition _from;
        private readonly Countdown _time = new Countdown();

        internal LeavingLeg(RuinDefinition from, int ticks)
        {
            _from = from;
            _time.Start(ticks);
        }

        public override RuinDefinition RuinBehind => _from;

        internal override void Tick(World world, Journey journey)
        {
            if (_time.CountDownOne())
            {
                journey.MoveOn();
                journey.Leg.Tick(world, journey);
            }
        }
    }
}
