using System.Collections.Generic;
using Scrapwalker.Game.Definitions;
using UnityEngine;

namespace Scrapwalker.Game.MainView.Effects
{
    public sealed class ParticleEffects : MonoBehaviour
    {
        private readonly List<ParticleSystem> _all = new List<ParticleSystem>();
        private readonly Dictionary<float, ParticleSystem> _sparks = new Dictionary<float, ParticleSystem>();
        private ParticleSystem _sparkPrefab;
        private readonly Dictionary<PuffKind, ParticleSystem> _puffs = new Dictionary<PuffKind, ParticleSystem>();
        private ParticleSystem _burst;
        private ParticleSystem _ring;
        private ParticleSystem _motes;
        private Color _moteTint = Color.clear;
        private float _framesPerSecond;

        public void Initialize(float framesPerSecond, ParticleSet particles)
        {
            _framesPerSecond = framesPerSecond;
            _sparkPrefab = particles.Spark;
            foreach (PuffKind kind in PuffKind.All)
            {
                _puffs[kind] = Spawn(kind.PrefabIn(particles));
            }

            _burst = Spawn(particles.Burst);
            _ring = Spawn(particles.Ring);
            _motes = Spawn(particles.Mote);
        }

        private ParticleSystem Spawn(ParticleSystem prefab)
        {
            ParticleSystem particleSystem = Instantiate(prefab, transform, false);
            particleSystem.name = prefab.name;
            _all.Add(particleSystem);
            return particleSystem;
        }

        private ParticleSystem SparkSystem(float gravity)
        {
            if (!_sparks.TryGetValue(gravity, out ParticleSystem particleSystem))
            {
                if (_sparkPrefab == null)
                {
                    return null;
                }

                particleSystem = Instantiate(_sparkPrefab, transform, false);
                particleSystem.name = _sparkPrefab.name + " " + gravity;
                ParticleSystem.ForceOverLifetimeModule force = particleSystem.forceOverLifetime;
                force.enabled = gravity != 0f;
                force.space = ParticleSystemSimulationSpace.Local;
                force.x = 0f;
                force.y = gravity * _framesPerSecond * _framesPerSecond;
                force.z = 0f;
                _sparks[gravity] = particleSystem;
                _all.Add(particleSystem);
            }

            return particleSystem;
        }

        public void OrderMotes(int order)
        {
            OrderOf(_motes, order);
        }

        public void OrderEffects(int burst, int puffs, int ring)
        {
            foreach (ParticleSystem particleSystem in _all)
            {
                if (particleSystem != _motes)
                {
                    OrderOf(particleSystem, particleSystem == _burst ? burst : particleSystem == _ring ? ring : puffs);
                }
            }
        }

        private static void OrderOf(ParticleSystem particleSystem, int order)
        {
            particleSystem.GetComponent<ParticleSystemRenderer>().sortingOrder = order;
        }

        public void Spark(Vector2 at, Vector2 velocity, float lifeFrames, in SparkLook look)
        {
            ParticleSystem particleSystem = SparkSystem(look.Gravity);
            if (particleSystem == null)
            {
                return;
            }

            float half = look.Size * 0.5f;
            var emitParams = new ParticleSystem.EmitParams
            {
                position = new Vector3(at.x + half, at.y + half, 0f),
                velocity = velocity * _framesPerSecond,
                startLifetime = lifeFrames / _framesPerSecond,
                startSize = look.Size,
                startColor = look.Color,
                applyShapeToPosition = false,
            };
            particleSystem.Emit(emitParams, 1);
        }

        public void Puff(Vector2 at, Vector2 velocity, float lifeFrames, float radius, PuffKind kind)
        {
            ParticleSystem particleSystem = _puffs[kind];
            if (particleSystem == null)
            {
                return;
            }

            Color color = kind.ColorFor(lifeFrames);
            var emitParams = new ParticleSystem.EmitParams
            {
                position = new Vector3(at.x, at.y, 0f),
                velocity = velocity * _framesPerSecond,
                startLifetime = lifeFrames / _framesPerSecond,
                startSize = radius * 2f,
                startColor = color,
                applyShapeToPosition = false,
            };
            particleSystem.Emit(emitParams, 1);
        }

        public void Burst(Vector2 at)
        {
            if (_burst == null)
            {
                return;
            }

            _burst.Emit(new ParticleSystem.EmitParams { position = new Vector3(at.x, at.y, 0f), applyShapeToPosition = false }, 1);
        }

        public void Ring(Vector2 at, float radius, float alpha, float grow, float fade, Color color)
        {
            if (_ring == null || fade <= 0f)
            {
                return;
            }

            float lifeFrames = alpha / fade;
            float maxRadius = radius + grow * lifeFrames;
            int n = Mathf.Clamp(Mathf.CeilToInt(2f * Mathf.PI * maxRadius / 3f), 24, 720);
            color.a = alpha;
            for (int i = 0; i < n; i++)
            {
                float t = i * 2f * Mathf.PI / n;
                var direction = new Vector3(Mathf.Cos(t), Mathf.Sin(t), 0f);
                _ring.Emit(new ParticleSystem.EmitParams
                {
                    position = new Vector3(at.x, at.y, 0f) + direction * radius,
                    velocity = direction * (grow * _framesPerSecond),
                    startLifetime = lifeFrames / _framesPerSecond,
                    startColor = color,
                    applyShapeToPosition = false,
                }, 1);
            }
        }

        public void TintMotes(Color color)
        {
            if (_motes == null || color == _moteTint)
            {
                return;
            }

            _moteTint = color;
            ParticleSystem.ColorOverLifetimeModule colorOverLifetime = _motes.colorOverLifetime;
            colorOverLifetime.enabled = true;
            colorOverLifetime.color = new ParticleSystem.MinMaxGradient(color);
        }

        public void Clear()
        {
            foreach (ParticleSystem particleSystem in _all)
            {
                if (particleSystem != _motes)
                {
                    particleSystem.Clear();
                }
            }
        }

        public void ClearRings()
        {
            if (_ring != null)
            {
                _ring.Clear();
            }
        }

        public void Advance(float seconds)
        {
            foreach (ParticleSystem particleSystem in _all)
            {
                if (!particleSystem.isPaused)
                {
                    particleSystem.Pause();
                }

                if (seconds > 0f)
                {
                    particleSystem.Simulate(seconds, true, false, false);
                }
            }
        }
    }
}
