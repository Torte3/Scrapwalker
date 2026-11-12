using System;
using UnityEngine;

namespace Scrapwalker.Game.Audio.Sounds
{
    [Serializable]
    public sealed class RepeatingSound
    {
        [SerializeField] private SoundCue _loop = new SoundCue();
        [SerializeField] private SoundCue _accent = new SoundCue();
        [SerializeField] private float _accentMinSeconds;
        [SerializeField] private float _accentMaxSeconds;

        public SoundCue Loop => _loop;
        public SoundCue Accent => _accent;
        public float AccentMinSeconds => _accentMinSeconds;
        public float AccentMaxSeconds => _accentMaxSeconds;
    }
}
