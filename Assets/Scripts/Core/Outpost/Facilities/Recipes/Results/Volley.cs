using Scrapwalker.Core.Framework;

namespace Scrapwalker.Core.Outpost.Facilities.Recipes.Results
{
    public sealed class Volley
    {
        internal Volley(int roundsLeft)
        {
            RoundsLeft = roundsLeft;
        }

        public int RoundsLeft { get; internal set; }
        public Countdown RoundTime { get; } = new Countdown();

        internal VolleyMemento CreateMemento()
        {
            return new VolleyMemento { RoundsLeft = RoundsLeft, RoundTime = RoundTime.CreateMemento() };
        }

        internal static Volley Restore(VolleyMemento memento)
        {
            if (memento == null)
            {
                return null;
            }

            var volley = new Volley(memento.RoundsLeft);
            volley.RoundTime.Restore(memento.RoundTime);
            return volley;
        }
    }
}
