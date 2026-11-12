using System;
using UnityEngine;

namespace Scrapwalker.Game.Look.Art.Parts
{
    [Serializable]
    public struct ArtOpening
    {
        [SerializeField] private bool _opens;
        [SerializeField] private float _openFrom;
        [SerializeField] private float _openTo;
        [SerializeField] private float _closeFrom;
        [SerializeField] private float _closeTo;

        public ArtOpening(float openFrom, float openTo, float closeFrom, float closeTo)
        {
            _opens = true;
            _openFrom = openFrom;
            _openTo = openTo;
            _closeFrom = closeFrom;
            _closeTo = closeTo;
        }

        public bool Opens => _opens;

        public float At(float t, float endFrames)
        {
            return Pen.Ramp(t, _openFrom, _openTo) * (1f - Pen.Ramp(t, endFrames + _closeFrom, endFrames + _closeTo));
        }
    }
}
