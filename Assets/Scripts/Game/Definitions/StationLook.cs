using System;
using UnityEngine;

namespace Scrapwalker.Game.Definitions
{
    [Serializable]
    public sealed class StationLook
    {
        [SerializeField] private float _dx;
        [SerializeField] private bool _onDeck;

        public float Dx => _dx;
        public bool OnDeck => _onDeck;
    }
}
