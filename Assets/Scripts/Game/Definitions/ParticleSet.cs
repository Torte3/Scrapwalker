using System;
using UnityEngine;

namespace Scrapwalker.Game.Definitions
{
    [Serializable]
    public sealed class ParticleSet
    {
        [SerializeField] private ParticleSystem _spark;
        [SerializeField] private ParticleSystem _steam;
        [SerializeField] private ParticleSystem _smoke;
        [SerializeField] private ParticleSystem _burst;
        [SerializeField] private ParticleSystem _ring;
        [SerializeField] private ParticleSystem _mote;

        public ParticleSystem Spark => _spark;
        public ParticleSystem Steam => _steam;
        public ParticleSystem Smoke => _smoke;
        public ParticleSystem Burst => _burst;
        public ParticleSystem Ring => _ring;
        public ParticleSystem Mote => _mote;
    }
}
