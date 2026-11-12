using System;
using UnityEngine;

namespace Scrapwalker.Game.Look.Art.Sets
{
    [Serializable]
    public sealed class LineArt
    {
        [SerializeField] private ArtLayer _glow;
        [SerializeField] private ArtLayer _core;

        public ArtLayer Glow => _glow;
        public ArtLayer Core => _core;
    }
}
