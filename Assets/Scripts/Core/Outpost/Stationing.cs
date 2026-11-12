namespace Scrapwalker.Core.Outpost
{
    public readonly struct Stationing
    {
        public Stationing(bool inPeace, bool inCombat)
        {
            InPeace = inPeace;
            InCombat = inCombat;
        }

        public bool InPeace { get; }
        public bool InCombat { get; }

        public Stationing WithPeace(bool on) => new Stationing(on, InCombat);
        public Stationing WithCombat(bool on) => new Stationing(InPeace, on);

        internal StationingMemento CreateMemento()
        {
            return new StationingMemento { InPeace = InPeace, InCombat = InCombat };
        }

        internal static Stationing Restore(StationingMemento memento)
        {
            return new Stationing(memento.InPeace, memento.InCombat);
        }

        public bool IsOn(bool inCombat) => inCombat ? InCombat : InPeace;
    }
}
