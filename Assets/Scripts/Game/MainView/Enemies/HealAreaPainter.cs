using Scrapwalker.Game.Look;
using UnityEngine;

namespace Scrapwalker.Game.MainView.Enemies
{
    public sealed class HealAreaPainter : WorldPainter
    {
        private const float BandHeight = 22f;
        private const float BandSwellHeight = 14f;
        private const float BandAlpha = 0.3f;
        private const float BandSwellAlpha = 0.25f;
        private const float SwellFrames = 30f;
        private const float LineAlpha = 0.5f;
        private const float LineSwellAlpha = 0.4f;
        private const float SoilGlowDepth = 6f;
        private const float SoilGlowShare = 0.35f;
        private const float EdgeHeight = 30f;
        private const float EdgeHaloHeight = 22f;
        private const float RunFrames = 13f;
        private const float RunHeight = 26f;
        private const float RunGlowRadius = 10f;
        private const float EyeFrames = 18f;
        private const float EyeGlowRadius = 14f;
        private const float EyeGlowMinRadius = 4f;

        private readonly ArtParts _parts;

        public HealAreaPainter(WorldScene scene, ArtParts parts)
            : base(scene)
        {
            _parts = parts;
        }

        public void DrawBack(float x0, float x1, float groundY, float? sinceHeal)
        {
            float swell = Swell(sinceHeal);
            Color glow = Palette.HealGlow;
            Color core = Palette.HealCore;
            _parts.Rise(x0, x1, groundY, BandHeight + BandSwellHeight * swell, glow.WithAlpha(BandAlpha + BandSwellAlpha * swell), DrawBand.Enemies);
            float line = LineAlpha + LineSwellAlpha * swell;
            _parts.FillRect(x0, groundY - 0.5f, x1 - x0, 1.5f, core.WithAlpha(line), DrawBand.Enemies);
            _parts.Rise(x0, x1, groundY, -SoilGlowDepth, glow.WithAlpha(line * SoilGlowShare), DrawBand.Enemies);
        }

        public void DrawFront(float x0, float x1, float groundY, float centerX, Vector2 eye, float? framesSinceHeal)
        {
            DrawEdge(x0, groundY);
            DrawEdge(x1, groundY);
            if (!framesSinceHeal.HasValue)
            {
                return;
            }

            float sinceHeal = framesSinceHeal.Value;
            if (sinceHeal < RunFrames)
            {
                float q = Pen.Ease(sinceHeal / RunFrames);
                DrawRunner(Mathf.Lerp(centerX, x0, q), groundY);
                DrawRunner(Mathf.Lerp(centerX, x1, q), groundY);
            }

            if (sinceHeal < EyeFrames)
            {
                float r = EyeGlowRadius * (1f - sinceHeal / EyeFrames) + EyeGlowMinRadius;
                _parts.Glow(eye.x, eye.y, r, r, Palette.HealGlow.WithAlpha(0.8f), DrawBand.Effects);
            }
        }

        private void DrawEdge(float x, float groundY)
        {
            _parts.Rise(x - 1f, x + 1f, groundY, EdgeHeight, Palette.HealCore.WithAlpha(0.8f), DrawBand.Effects);
            _parts.Rise(x - 3f, x + 3f, groundY, EdgeHaloHeight, Palette.HealGlow.WithAlpha(0.25f), DrawBand.Effects);
            _parts.FillRect(x - 2.5f, groundY - 1f, 5f, 2f, Palette.HealCore.WithAlpha(0.9f), DrawBand.Effects);
        }

        private void DrawRunner(float x, float groundY)
        {
            _parts.Glow(x, groundY, RunGlowRadius, RunGlowRadius, Palette.HealGlow.WithAlpha(0.9f), DrawBand.Effects);
            _parts.Rise(x - 1.5f, x + 1.5f, groundY, RunHeight, Palette.HealCore.WithAlpha(0.9f), DrawBand.Effects);
        }

        private static float Swell(float? sinceHeal)
        {
            return sinceHeal.HasValue && sinceHeal.Value < SwellFrames ? 1f - sinceHeal.Value / SwellFrames : 0f;
        }
    }
}
