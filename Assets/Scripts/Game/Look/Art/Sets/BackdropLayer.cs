using System;
using Scrapwalker.Core.Root;
using UnityEngine;

namespace Scrapwalker.Game.Look.Art.Sets
{
    [Serializable]
    public sealed class BackdropLayer
    {
        [SerializeField] private ArtLayer[] _times;
        [SerializeField] private float _parallax;
        [SerializeField] private float _period;
        [SerializeField] private bool _fadesByWeight;

        public float Parallax => _parallax;
        public float Period => _period;
        public bool IsStrip => _period <= 0f;
        public bool FadesByWeight => _fadesByWeight;

        public ArtLayer At(TimeOfDay time)
        {
            return _times[(int)time];
        }
    }
}
