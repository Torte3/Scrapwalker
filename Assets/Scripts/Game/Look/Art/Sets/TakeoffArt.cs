using System;
using UnityEngine;

namespace Scrapwalker.Game.Look.Art.Sets
{
    [Serializable]
    public sealed class TakeoffArt
    {
        [SerializeField] private ArtLayer _booster;
        [SerializeField] private ArtLayer _doorEdge;
        [SerializeField] private ArtLayer _doorMiddle;
        [SerializeField] private ArtLayer _flame;
        [SerializeField] private ArtLayer _tire;
        [SerializeField] private ArtLayer _spokes;

        public ArtLayer Booster => _booster;
        public ArtLayer DoorEdge => _doorEdge;
        public ArtLayer DoorMiddle => _doorMiddle;
        public ArtLayer Flame => _flame;
        public ArtLayer Tire => _tire;
        public ArtLayer Spokes => _spokes;
    }
}
