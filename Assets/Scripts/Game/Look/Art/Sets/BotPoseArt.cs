using System;
using UnityEngine;

namespace Scrapwalker.Game.Look.Art.Sets
{
    [Serializable]
    public sealed class BotPoseArt
    {
        [SerializeField] private ArtEntry _normal;
        [SerializeField] private ArtEntry _lowPower;
        [SerializeField] private ArtEntry _lost;

        public ArtEntry Normal => _normal;
        public ArtEntry LowPower => _lowPower;
        public ArtEntry Lost => _lost;
    }
}
