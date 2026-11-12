using System;
using UnityEngine;

namespace Scrapwalker.Game.Look.Art.Sets
{
    [Serializable]
    public sealed class EffectArt
    {
        [SerializeField] private ArtLayer _disc;
        [SerializeField] private ArtLayer _glow;
        [SerializeField] private ArtLayer _shadow;
        [SerializeField] private ArtLayer _white;
        [SerializeField] private ArtLayer _rise;
        [SerializeField] private ArtLayer _pullArc;
        [SerializeField] private LineArt _shotLine = new LineArt();
        [SerializeField] private LineArt _bolt = new LineArt();
        [SerializeField] private ArtLayer _shellTail;
        [SerializeField] private ArtLayer _shellBody;
        [SerializeField] private ArtLayer _lob;
        [SerializeField] private ArtLayer _stunGlow;
        [SerializeField] private ArtLayer _stunSparks;
        [SerializeField] private ArtLayer _inboundChunk;
        [SerializeField] private ArtLayer _inboundMark;

        public ArtLayer Disc => _disc;
        public ArtLayer Glow => _glow;
        public ArtLayer Shadow => _shadow;
        public ArtLayer White => _white;
        public ArtLayer Rise => _rise;
        public ArtLayer PullArc => _pullArc;
        public LineArt ShotLine => _shotLine;
        public LineArt Bolt => _bolt;
        public ArtLayer ShellTail => _shellTail;
        public ArtLayer ShellBody => _shellBody;
        public ArtLayer Lob => _lob;
        public ArtLayer StunGlow => _stunGlow;
        public ArtLayer StunSparks => _stunSparks;
        public ArtLayer InboundChunk => _inboundChunk;
        public ArtLayer InboundMark => _inboundMark;
    }
}
