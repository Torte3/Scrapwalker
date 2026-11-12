using System;
using UnityEngine;

namespace Scrapwalker.Game.Audio.Sounds
{
    [Serializable]
    public sealed class SoundCue
    {
        [SerializeField] private AudioClip _clip;
        [SerializeField] private float _volumeDecibels;
        [SerializeField] private float _pitchJitter;
        [SerializeField] private int _maxVoices;

        public AudioClip Clip => _clip;
        public float Volume => Mathf.Pow(10f, _volumeDecibels / 20f);
        public float PitchJitter => _pitchJitter;
        public int MaxVoices => _maxVoices;
        public bool IsEmpty => _clip == null;
    }
}
