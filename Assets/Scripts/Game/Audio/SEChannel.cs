using System;
using UnityEngine;
using UnityEngine.Audio;

namespace Scrapwalker.Game.Audio
{
    [Serializable]
    public sealed class SEChannel
    {
        [SerializeField] private AudioMixerGroup _shiftedGroup;
        [SerializeField] private PitchShiftParameters _pitch;
        [SerializeField] private float _rate;

        public AudioMixerGroup ShiftedGroup => _shiftedGroup;
        public PitchShiftParameters Pitch => _pitch;
        public float Rate => _rate;
    }
}
