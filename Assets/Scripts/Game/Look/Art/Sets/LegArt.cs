using System;
using UnityEngine;

namespace Scrapwalker.Game.Look.Art.Sets
{
    [Serializable]
    public sealed class LegArt
    {
        [SerializeField] private ArtLayer _thigh;
        [SerializeField] private ArtLayer _shin;
        [SerializeField] private ArtLayer _cylinderRod;
        [SerializeField] private ArtLayer _cylinderTube;
        [SerializeField] private ArtLayer _knee;
        [SerializeField] private ArtLayer _ankle;
        [SerializeField] private ArtLayer _foot;
        [SerializeField] private ArtLayer _hip;

        public ArtLayer Thigh => _thigh;
        public ArtLayer Shin => _shin;
        public ArtLayer CylinderRod => _cylinderRod;
        public ArtLayer CylinderTube => _cylinderTube;
        public ArtLayer Knee => _knee;
        public ArtLayer Ankle => _ankle;
        public ArtLayer Foot => _foot;
        public ArtLayer Hip => _hip;
    }
}
