using System;
using UnityEngine;

namespace Scrapwalker.Game.Look.Art.Parts
{
    [Serializable]
    public struct ArtBump
    {
        [SerializeField] private float _amplitude;
        [SerializeField] private float _rate;

        public ArtBump(float amplitude, float rate)
        {
            _amplitude = amplitude;
            _rate = rate;
        }

        public float At(float t)
        {
            return Mathf.Abs(Mathf.Sin(t * _rate)) * _amplitude;
        }
    }
}
