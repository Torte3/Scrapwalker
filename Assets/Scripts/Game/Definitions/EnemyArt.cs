using System;
using Scrapwalker.Game.Look.Art;
using UnityEngine;

namespace Scrapwalker.Game.Definitions
{
    [Serializable]
    public sealed class EnemyArt
    {
        [SerializeField] private ArtEntry _calm;
        [SerializeField] private ArtEntry _alert;
        [SerializeField] private ArtEntry _attack;
        [SerializeField] private ArtLayer _body;
        [SerializeField] private Vector2[] _booms;

        public ArtEntry Calm => _calm;
        public ArtEntry Alert => _alert;
        public ArtEntry Attack => _attack;
        public ArtLayer Body => _body;
        public Vector2[] Booms => _booms;
    }
}
