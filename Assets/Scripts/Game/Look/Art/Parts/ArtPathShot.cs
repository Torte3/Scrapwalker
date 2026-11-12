using System;
using UnityEngine;

namespace Scrapwalker.Game.Look.Art.Parts
{
    [Serializable]
    public sealed class ArtPathShot : ArtPart
    {
        [SerializeField] private Vector2[] _mouths;
        [SerializeField] private float _flyFrames;
        [SerializeField] private float _arc;
        [SerializeField] private Color _color;
        [SerializeField] private Color _core;
        [SerializeField] private bool _hasTrail;
        [SerializeField] private int _trailCount;
        [SerializeField] private float _trailSpacing;
        [SerializeField] private float _trailRadius;
        [SerializeField] private float _trailGrowth;
        [SerializeField] private float _trailAlpha;
        [SerializeField] private float _trailFade;
        [SerializeField] private bool _hasGlow;
        [SerializeField] private float _glowFrom;
        [SerializeField] private float _glowRadius;
        [SerializeField] private float _glowAlpha;

        public ArtPathShot(Vector2[] mouths, float flyFrames, float arc, Color color, Color core)
        {
            _mouths = mouths;
            _flyFrames = flyFrames;
            _arc = arc;
            _color = color;
            _core = core;
        }

        public float FlyFrames => _flyFrames;

        public override bool DrawsItself => true;

        public override bool DrawsAboveFigures => true;

        public void Trail(int count, float spacing, float radius, float growth, float alpha, float fade)
        {
            _hasTrail = true;
            _trailCount = count;
            _trailSpacing = spacing;
            _trailRadius = radius;
            _trailGrowth = growth;
            _trailAlpha = alpha;
            _trailFade = fade;
        }

        public void Glow(float from, float radius, float alpha)
        {
            _hasGlow = true;
            _glowFrom = from;
            _glowRadius = radius;
            _glowAlpha = alpha;
        }

        public override void Draw(IArtStage stage, ArtState state, ArtLayer layer)
        {
            if (_flyFrames <= 0f || _mouths.Length == 0)
            {
                return;
            }

            if (_hasTrail)
            {
                ForEachShot(stage, state, (from, target, progress) => DrawTrail(stage, from, target, progress));
            }

            ForEachShot(stage, state, (from, target, progress) => DrawShell(stage, layer, from, target, progress));
            if (_hasGlow)
            {
                ForEachShot(stage, state, (from, target, progress) => DrawGlow(stage, from, target, progress));
            }
        }

        private void ForEachShot(IArtStage stage, ArtState state, Action<Vector2, Vector2, float> draw)
        {
            int shots = state.ShotCount;
            for (int i = 0; i < shots; i++)
            {
                if (!state.TryGetShotCell(i, out Vector2 cell))
                {
                    break;
                }

                float progress = (state.MoveFrames - state.LaunchFrame(i)) / _flyFrames;
                if (progress < 0f || progress >= 1f)
                {
                    continue;
                }

                draw(_mouths[(int)Mathf.Repeat(i - shots, _mouths.Length)], stage.FromCanvas(cell), progress);
            }
        }

        private void DrawShell(IArtStage stage, ArtLayer layer, Vector2 from, Vector2 target, float progress)
        {
            Vector2 placed = Along(from, target, progress);
            Vector2 heading = target - from + new Vector2(0f, -4f * _arc * (1f - 2f * progress));
            stage.Save();
            stage.Translate(placed.x, placed.y);
            stage.Rotate(Mathf.Atan2(heading.y, heading.x) - Mathf.PI);
            stage.Translate(-_mouths[0].x, -_mouths[0].y);
            stage.Put(layer, layer.First, Color.white);
            stage.Restore();
        }

        private void DrawGlow(IArtStage stage, Vector2 from, Vector2 target, float progress)
        {
            if (progress > _glowFrom)
            {
                Vector2 placed = Along(from, target, progress);
                stage.Glow(placed.x, placed.y, _glowRadius, _glowRadius, _core.WithAlpha(_glowAlpha));
            }
        }

        private void DrawTrail(IArtStage stage, Vector2 from, Vector2 target, float progress)
        {
            for (int s = 1; s <= _trailCount; s++)
            {
                float back = progress - s * _trailSpacing;
                if (back < 0f)
                {
                    break;
                }

                Vector2 q = Along(from, target, back);
                stage.Disc(q.x, q.y, _trailRadius + s * _trailGrowth, _color.WithAlpha(_trailAlpha - s * _trailFade));
            }
        }

        private Vector2 Along(Vector2 from, Vector2 target, float k)
        {
            return Vector2.Lerp(from, target, k) + new Vector2(0f, -4f * _arc * k * (1f - k));
        }
    }
}
