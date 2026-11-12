using System;
using Scrapwalker.Game.Look.Art;
using UnityEngine;

namespace Scrapwalker.Game.Definitions
{
    [Serializable]
    public sealed class TimeOfDayLook
    {
        [SerializeField] private Color _skyTop;
        [SerializeField] private Color _ground;
        [SerializeField] private Color _motes;
        [SerializeField] private float _stars;
        [SerializeField] private TimeOfDayArt _art = new TimeOfDayArt();

        public Color SkyTop => _skyTop;
        public Color Ground => _ground;
        public Color Motes => _motes;
        public float Stars => _stars;
        public TimeOfDayArt Art => _art;
    }
}
