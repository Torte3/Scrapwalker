using System;
using UnityEngine;

namespace Scrapwalker.Game.Look.Art
{
    [Serializable]
    public struct ArtWave
    {
        [SerializeField] private float _center;
        [SerializeField] private float _spin;
        [SerializeField] private float _idle;
        [SerializeField] private float _swayAmplitude;
        [SerializeField] private float _swayRate;
        [SerializeField] private float _swayPhase;

        public ArtWave(float center, float spin, float idle, float swayAmplitude, float swayRate, float swayPhase)
        {
            _center = center;
            _spin = spin;
            _idle = idle;
            _swayAmplitude = swayAmplitude;
            _swayRate = swayRate;
            _swayPhase = swayPhase;
        }

        public float At(bool isActive, float t)
        {
            return isActive ? _center + _spin * t + _swayAmplitude * Mathf.Sin(_swayRate * t + _swayPhase) : _idle;
        }
    }
}
