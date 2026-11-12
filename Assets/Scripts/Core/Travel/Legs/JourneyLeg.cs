using Scrapwalker.Core.Framework;
using Scrapwalker.Core.Root;

namespace Scrapwalker.Core.Travel.Legs
{
    public abstract class JourneyLeg
    {
        private static readonly Countdown NoRoute = new Countdown();

        public virtual bool IsTraveling => true;
        public virtual RuinDefinition CurrentRuin => null;
        public virtual RuinDefinition RuinBehind => null;
        public virtual Countdown Route => NoRoute;

        public virtual RuinDefinition RuinAhead(Journey journey)
        {
            return null;
        }

        internal virtual JourneyLeg WithoutDestination => this;

        internal abstract void Tick(World world, Journey journey);
    }
}
