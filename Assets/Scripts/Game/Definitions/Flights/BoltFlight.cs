using System;
using UnityEngine;

namespace Scrapwalker.Game.Definitions.Flights
{
    [Serializable]
    public sealed class BoltFlight : ShotFlight
    {
        [SerializeField] private Color _lineColor;
        [SerializeField] private Color _lineCore;

        public Color LineColor => _lineColor;
        public Color LineCore => _lineCore;

        public override T Show<T>(IShotFlightView<T> view)
        {
            return view.Bolt(this);
        }
    }
}
