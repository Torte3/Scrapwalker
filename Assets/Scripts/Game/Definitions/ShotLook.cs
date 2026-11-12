using System;
using UnityEngine;

namespace Scrapwalker.Game.Definitions
{
    [Serializable]
    public sealed class ShotLook
    {
        [SerializeReference] private ShotFlight _flight;
        [SerializeField] private float _muzzleAnchorY;
        [SerializeField] private float _muzzleOffsetY;
        [SerializeField] private float _hitHeight;
        [SerializeField] private SparkBurst _muzzle;
        [SerializeField] private SparkBurst _hit;

        public ShotFlight Flight => _flight;
        public float HitHeight => _hitHeight;
        public SparkBurst Muzzle => _muzzle;
        public SparkBurst Hit => _hit;

        public float MuzzleY(float top, float height)
        {
            return top + height * _muzzleAnchorY + _muzzleOffsetY;
        }
    }
}
