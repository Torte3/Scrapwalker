using System;
using UnityEngine;

namespace Scrapwalker.Game.Definitions.Flights
{
    [Serializable]
    public sealed class LobFlight : ShotFlight
    {
        [SerializeField] private float _arc;
        [SerializeField] private float _arcRun;

        public override float ArcFor(float run)
        {
            return _arcRun > 0f ? _arc * Mathf.Min(1f, run / _arcRun) : _arc;
        }

        public override T Show<T>(IShotFlightView<T> view)
        {
            return view.Lob(this);
        }
    }
}
