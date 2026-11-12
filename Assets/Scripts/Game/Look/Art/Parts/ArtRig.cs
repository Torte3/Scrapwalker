using System;
using UnityEngine;

namespace Scrapwalker.Game.Look.Art.Parts
{
    [Serializable]
    public sealed class ArtRig : ArtPart
    {
        [SerializeField] private Vector2[] _points;
        [SerializeField] private float _length;
        [SerializeField] private float _along;
        [SerializeField] private float _grip;

        public ArtRig(Vector2[] points, float length, float along, float grip)
        {
            _points = points;
            _length = length;
            _along = along;
            _grip = grip;
        }

        public Vector2 At => _points.Length > 0 ? _points[0] : Vector2.zero;
        public Vector2[] Points => _points;
        public float Length => _length;
        public float Along => _along;
        public float Grip => _grip;
    }
}
