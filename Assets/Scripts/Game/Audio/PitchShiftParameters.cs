using System;
using UnityEngine;

namespace Scrapwalker.Game.Audio
{
    [Serializable]
    public sealed class PitchShiftParameters
    {
        [SerializeField] private string _first;
        [SerializeField] private string _second;

        public string First => _first;
        public string Second => _second;
    }
}
