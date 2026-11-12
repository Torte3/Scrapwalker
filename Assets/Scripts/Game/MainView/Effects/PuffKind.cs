using System;
using Scrapwalker.Game.Definitions;
using Scrapwalker.Game.Look;
using UnityEngine;

namespace Scrapwalker.Game.MainView.Effects
{
    public sealed class PuffKind
    {
        public static readonly PuffKind Steam = new PuffKind(Palette.PuffSteam, 0.45f, 40f, set => set.Steam);
        public static readonly PuffKind Smoke = new PuffKind(Palette.PuffSmoke, 0.5f, 100f, set => set.Smoke);
        public static readonly PuffKind[] All = { Steam, Smoke };

        private readonly Color _color;
        private readonly float _maxAlpha;
        private readonly float _fadeInFrames;
        private readonly Func<ParticleSet, ParticleSystem> _prefab;

        private PuffKind(Color color, float maxAlpha, float fadeInFrames, Func<ParticleSet, ParticleSystem> prefab)
        {
            _color = color;
            _maxAlpha = maxAlpha;
            _fadeInFrames = fadeInFrames;
            _prefab = prefab;
        }

        public ParticleSystem PrefabIn(ParticleSet particles)
        {
            return _prefab(particles);
        }

        public Color ColorFor(float lifeFrames)
        {
            return _color.WithAlpha(Mathf.Min(1f, lifeFrames / _fadeInFrames) * _maxAlpha);
        }
    }
}
