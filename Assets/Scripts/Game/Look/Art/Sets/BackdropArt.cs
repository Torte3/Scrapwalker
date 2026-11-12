using System;
using UnityEngine;

namespace Scrapwalker.Game.Look.Art.Sets
{
    [Serializable]
    public sealed class BackdropArt
    {
        [SerializeField] private ArtLayer _ground;
        [SerializeField] private ArtLayer _hill;
        [SerializeField] private ArtLayer[] _props;
        [SerializeField] private ArtLayer _rubble;
        [SerializeField] private ArtLayer _space;
        [SerializeField] private BackdropLayer[] _far;

        public ArtLayer Ground => _ground;
        public ArtLayer Hill => _hill;
        public ArtLayer[] Props => _props;
        public ArtLayer Rubble => _rubble;
        public ArtLayer Space => _space;
        public BackdropLayer[] Far => _far;
    }
}
