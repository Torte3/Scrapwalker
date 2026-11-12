using System;
using UnityEngine;

namespace Scrapwalker.Game.Look.Art.Parts
{
    [Serializable]
    public sealed class ArtFire : ArtPart
    {
        [SerializeField] private float _phase;

        public ArtFire(float phase)
        {
            _phase = phase;
        }

        public float Phase => _phase;
    }
}
