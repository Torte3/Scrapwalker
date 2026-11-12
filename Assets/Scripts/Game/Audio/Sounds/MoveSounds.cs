using System;
using UnityEngine;

namespace Scrapwalker.Game.Audio.Sounds
{
    [Serializable]
    public sealed class MoveSounds
    {
        [SerializeField] private SoundCue _begin = new SoundCue();
        [SerializeField] private SoundCue _hit = new SoundCue();

        public SoundCue Begin => _begin;
        public SoundCue Hit => _hit;
    }
}
