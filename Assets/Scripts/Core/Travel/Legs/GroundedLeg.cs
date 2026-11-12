using Scrapwalker.Core.Root;

namespace Scrapwalker.Core.Travel.Legs
{
    public sealed class GroundedLeg : JourneyLeg
    {
        private readonly RuinDefinition _ruin;

        internal GroundedLeg(RuinDefinition ruin)
        {
            _ruin = ruin;
        }

        public override bool IsTraveling => false;
        public override RuinDefinition CurrentRuin => _ruin;

        public override RuinDefinition RuinAhead(Journey journey)
        {
            return _ruin;
        }

        internal override void Tick(World world, Journey journey)
        {
        }
    }
}
