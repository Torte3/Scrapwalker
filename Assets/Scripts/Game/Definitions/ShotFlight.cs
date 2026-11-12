using System;

namespace Scrapwalker.Game.Definitions
{
    [Serializable]
    public abstract class ShotFlight
    {
        public virtual float ArcFor(float run)
        {
            return 0f;
        }

        public abstract T Show<T>(IShotFlightView<T> view);
    }
}
