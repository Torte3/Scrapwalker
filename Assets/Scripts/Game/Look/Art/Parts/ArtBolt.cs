using System;
using UnityEngine;

namespace Scrapwalker.Game.Look.Art.Parts
{
    [Serializable]
    public sealed class ArtBolt : ArtPart
    {
        [SerializeField] private Vector2 _from;
        [SerializeField] private Color _color;
        [SerializeField] private Color _core;

        public ArtBolt(Vector2 from, Color color, Color core)
        {
            _from = from;
            _color = color;
            _core = core;
        }

        public Vector2 From => _from;
        public Color Color => _color;
        public Color Core => _core;

        public override bool DrawsItself => true;
    }
}
