using Scrapwalker.Core.Framework;
using Scrapwalker.Core.Root;
using Scrapwalker.Core.Travel.Legs;

namespace Scrapwalker.Core.Travel
{
    public sealed class Journey
    {
        public JourneyLeg Leg { get; private set; } = new GroundedLeg(null);
        public RuinDefinition Destination { get; private set; }
        public bool IsRepeating { get; internal set; }

        public RuinDefinition CurrentRuin => Leg.CurrentRuin;
        public bool IsTraveling => Leg.IsTraveling;
        public bool IsAtRuin => CurrentRuin != null;
        public RuinDefinition RuinBehind => Leg.RuinBehind;
        public RuinDefinition RuinAhead => Leg.RuinAhead(this);
        public Countdown Route => Leg.Route;

        internal void Depart(int leavingTicks)
        {
            Leg = new LeavingLeg(CurrentRuin, leavingTicks);
            IsRepeating = false;
        }

        internal void MoveOn()
        {
            Leg = new MovingLeg();
        }

        internal void HeadFor(RuinDefinition ruin, bool repeat)
        {
            Destination = ruin;
            IsRepeating = repeat;
        }

        internal void BeginArrival(int ticks)
        {
            Leg = new ArrivingLeg(ticks);
        }

        internal void DropDestination()
        {
            Leg = Leg.WithoutDestination;
            Destination = null;
            IsRepeating = false;
        }

        internal JourneyMemento CreateMemento()
        {
            return new JourneyMemento { Traveling = IsTraveling, Destination = Destination?.ID, Repeat = IsRepeating };
        }

        internal void Restore(JourneyMemento memento, LoadContext load)
        {
            Leg = memento.Traveling ? (JourneyLeg)new MovingLeg() : new GroundedLeg(null);
            Destination = load.FindOrNone<RuinDefinition>(memento.Destination);
            IsRepeating = memento.Repeat;
        }

        internal void Land(RuinDefinition ruin)
        {
            Leg = new GroundedLeg(ruin);
            Destination = null;
        }
    }
}
