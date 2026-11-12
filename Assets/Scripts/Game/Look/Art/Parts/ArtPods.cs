using System;
using UnityEngine;

namespace Scrapwalker.Game.Look.Art.Parts
{
    [Serializable]
    public sealed class ArtPods : ArtPart
    {
        [SerializeField] private ArtOpening _opening;
        [SerializeField] private int _pods;

        public ArtPods(ArtOpening opening, int pods)
        {
            _opening = opening;
            _pods = pods;
        }

        public ArtOpening Opening => _opening;
        public int Pods => _pods;
    }
}
