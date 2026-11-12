using System;
using Scrapwalker.Game.Look.Art;
using UnityEngine;

namespace Scrapwalker.Game.Definitions
{
    [Serializable]
    public sealed class TimeOfDayArt
    {
        public const int CloudCount = 8;

        [SerializeField] private ArtLayer _sky;
        [SerializeField] private ArtLayer _sun;
        [SerializeField] private ArtLayer[] _clouds;

        public ArtLayer Sky => _sky;
        public ArtLayer Sun => _sun;
        public ArtLayer[] Clouds => _clouds;
    }
}
