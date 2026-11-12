using System;
using UnityEngine;

namespace Scrapwalker.Game.Look.Art.Parts
{
    [Serializable]
    public sealed class ArtPuffs : ArtPart
    {
        [SerializeField] private Vector2 _at;
        [SerializeField] private Color _color;
        [SerializeField] private int _count;
        [SerializeField] private float _lifeFrames;
        [SerializeField] private Vector2 _jitter;
        [SerializeField] private Vector2 _drift;
        [SerializeField] private Vector2 _spread;
        [SerializeField] private float _startRadius;
        [SerializeField] private float _growth;
        [SerializeField] private float _growthRandom;
        [SerializeField] private float _alpha;
        [SerializeField] private float _fadeIn;

        public ArtPuffs(Vector2 at, Color color, int count, float lifeFrames, Vector2 jitter, Vector2 drift, Vector2 spread, Vector3 size, float alpha, float fadeIn)
        {
            _at = at;
            _color = color;
            _count = count;
            _lifeFrames = lifeFrames;
            _jitter = jitter;
            _drift = drift;
            _spread = spread;
            _startRadius = size.x;
            _growth = size.y;
            _growthRandom = size.z;
            _alpha = alpha;
            _fadeIn = fadeIn;
        }

        public override bool DrawsItself => true;

        public override void Draw(IArtStage stage, ArtState state, ArtLayer layer)
        {
            if (!state.Runs(layer))
            {
                return;
            }

            for (int i = 0; i < _count; i++)
            {
                float t = state.Clock + i * _lifeFrames / _count;
                float age = t % _lifeFrames / _lifeFrames;
                float id = Mathf.Floor(t / _lifeFrames) * _count + i;
                float r1 = Pen.Hash(id);
                float r2 = Pen.Hash(id + 0.5f);
                float x = _at.x + (r1 - 0.5f) * _jitter.x + age * (_drift.x + r1 * _spread.x);
                float y = _at.y + (r1 - 0.5f) * _jitter.y + age * (_drift.y + r2 * _spread.y);
                float radius = _startRadius + age * (_growth + r2 * _growthRandom);
                float fadeIn = _fadeIn > 0f ? Mathf.Min(1f, age * _fadeIn) : 1f;
                stage.Disc(x, y, radius, _color.WithAlpha(_alpha * (1f - age) * fadeIn));
            }
        }
    }
}
