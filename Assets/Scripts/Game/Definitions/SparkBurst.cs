using System;
using UnityEngine;

namespace Scrapwalker.Game.Definitions
{
    [Serializable]
    public struct SparkBurst
    {
        [SerializeField] private int _count;
        [SerializeField] private float _xSpread;
        [SerializeField] private float _ySpread;
        [SerializeField] private float _vxMin;
        [SerializeField] private float _vxMax;
        [SerializeField] private float _vyMin;
        [SerializeField] private float _vyMax;
        [SerializeField] private float _lifeFrames;
        [SerializeField] private float _lifeRandom;
        [SerializeField] private Color _color;
        [SerializeField] private float _size;
        [SerializeField] private float _sizeRandom;
        [SerializeField] private float _gravity;

        public SparkBurst(int count, Vector2 spread, Vector2 vx, Vector2 vy, float lifeFrames, float lifeRandom, Color color, float size, float sizeRandom, float gravity)
        {
            _count = count;
            _xSpread = spread.x;
            _ySpread = spread.y;
            _vxMin = vx.x;
            _vxMax = vx.y;
            _vyMin = vy.x;
            _vyMax = vy.y;
            _lifeFrames = lifeFrames;
            _lifeRandom = lifeRandom;
            _color = color;
            _size = size;
            _sizeRandom = sizeRandom;
            _gravity = gravity;
        }

        public int Count => _count;
        public float XSpread => _xSpread;
        public float YSpread => _ySpread;
        public float VxMin => _vxMin;
        public float VxMax => _vxMax;
        public float VyMin => _vyMin;
        public float VyMax => _vyMax;
        public float LifeFrames => _lifeFrames;
        public float LifeRandom => _lifeRandom;
        public Color Color => _color;
        public float Size => _size;
        public float SizeRandom => _sizeRandom;
        public float Gravity => _gravity;

        public SparkBurst Spreading(Vector2 spread)
        {
            SparkBurst copy = this;
            copy._xSpread = spread.x;
            copy._ySpread = spread.y;
            return copy;
        }

        public SparkBurst Counted(int count)
        {
            SparkBurst copy = this;
            copy._count = count;
            return copy;
        }

        public SparkBurst Colored(Color color)
        {
            SparkBurst copy = this;
            copy._color = color;
            return copy;
        }
    }
}
