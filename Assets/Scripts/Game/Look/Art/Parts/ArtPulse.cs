using System;
using UnityEngine;

namespace Scrapwalker.Game.Look.Art.Parts
{
    [Serializable]
    public sealed class ArtPulse : ArtPart
    {
        [SerializeField] private ArtWave _wave;

        public ArtPulse(ArtWave wave)
        {
            _wave = wave;
        }

        public override bool IsLight => true;

        public override float Alpha(float clock)
        {
            return _wave.At(true, clock);
        }
    }
}
