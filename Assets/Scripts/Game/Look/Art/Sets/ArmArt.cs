using System;
using UnityEngine;

namespace Scrapwalker.Game.Look.Art.Sets
{
    [Serializable]
    public sealed class ArmArt
    {
        [SerializeField] private ArtLayer _mount;
        [SerializeField] private ArtLayer _upper;
        [SerializeField] private ArtLayer _fore;
        [SerializeField] private ArtLayer _bundle;
        [SerializeField] private ArtLayer _clawTop;
        [SerializeField] private ArtLayer _clawBottom;
        [SerializeField] private ArtLayer _hose;
        [SerializeField] private ArtLayer _shoulderPin;
        [SerializeField] private ArtLayer _elbowPin;
        [SerializeField] private ArtLayer _cylinderRod;
        [SerializeField] private ArtLayer _cylinderTube;

        public ArtLayer Mount => _mount;
        public ArtLayer Upper => _upper;
        public ArtLayer Fore => _fore;
        public ArtLayer Bundle => _bundle;
        public ArtLayer ClawTop => _clawTop;
        public ArtLayer ClawBottom => _clawBottom;
        public ArtLayer Hose => _hose;
        public ArtLayer ShoulderPin => _shoulderPin;
        public ArtLayer ElbowPin => _elbowPin;
        public ArtLayer CylinderRod => _cylinderRod;
        public ArtLayer CylinderTube => _cylinderTube;
    }
}
