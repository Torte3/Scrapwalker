using System;
using UnityEngine;

namespace Scrapwalker.Game.Definitions.Flights
{
    [Serializable]
    public sealed class ShellFlight : ShotFlight
    {
        [SerializeField] private float _arc;

        public override float ArcFor(float run)
        {
            return _arc;
        }

        public override T Show<T>(IShotFlightView<T> view)
        {
            return view.Shell(this);
        }
    }
}
