using System;
using UnityEngine;

namespace Scrapwalker.Game.Look.Art.Parts
{
    [Serializable]
    public sealed class ArtCalm : ArtPart
    {
        [SerializeField] private float _rate;

        public ArtCalm(float rate)
        {
            _rate = rate;
        }

        public float Rate => _rate;
    }
}
