using System;
using UnityEngine;

namespace Scrapwalker.Game.Look.Art.Sets
{
    [Serializable]
    public sealed class OutpostArt
    {
        [SerializeField] private ArtEntry _hull;
        [SerializeField] private ArtEntry[] _radars;
        [SerializeField] private ArtLayer _rivet;
        [SerializeField] private ArtLayer _emptyCell;
        [SerializeField] private ArtLayer[] _cracks;
        [SerializeField] private ArtLayer _buildStripes;
        [SerializeField] private ArtLayer _dismantleStripes;

        public ArtEntry Hull => _hull;
        public ArtEntry[] Radars => _radars;
        public ArtLayer Rivet => _rivet;
        public ArtLayer EmptyCell => _emptyCell;
        public ArtLayer[] Cracks => _cracks;
        public ArtLayer BuildStripes => _buildStripes;
        public ArtLayer DismantleStripes => _dismantleStripes;
    }
}
