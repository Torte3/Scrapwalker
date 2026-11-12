using System;
using UnityEngine;

namespace Scrapwalker.Game.Definitions.Flights
{
    [Serializable]
    public sealed class TracerFlight : ShotFlight
    {
        [SerializeField] private Color _color;

        public Color Color => _color;

        public override T Show<T>(IShotFlightView<T> view)
        {
            return view.Tracer(this);
        }
    }
}
