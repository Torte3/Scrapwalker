using System;
using UnityEngine;

namespace Scrapwalker.Game.Look.Art.Parts
{
    [Serializable]
    public sealed class ArtBlink : ArtPart
    {
        [SerializeField] private float _period;
        [SerializeField] private float _on;

        public ArtBlink(float period, float on)
        {
            _period = period;
            _on = on;
        }

        public override bool IsLight => true;

        public override float Alpha(float clock)
        {
            return Mathf.Repeat(clock, _period) < _on ? 1f : 0f;
        }
    }
}
