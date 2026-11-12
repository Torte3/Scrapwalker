using System;
using System.Collections.Generic;
using Scrapwalker.Core.Root;
using Scrapwalker.Game.Definitions;
using Scrapwalker.Game.Look.Art;
using Scrapwalker.Game.Look.Art.Sets;
using UnityEngine;
using static Scrapwalker.Game.Look.Palette;
using static Scrapwalker.Game.MainView.WorldScene;

namespace Scrapwalker.Game.MainView.Backdrop
{
    public sealed class BackdropPainter : WorldPainter
    {

        private const float SkyHeight = 2000f;
        private const float UnderGroundHeight = 400f;
        private const int CloudSeed = 5;
        private const int StarSeed = 77;
        private const int Stars = 180;
        private const float StarsWidth = 2800f;
        private const float StarFadeY = 520f;
        private const float StarFadeSpan = 380f;

        private readonly ArtParts _parts;
        private readonly List<Cloud> _clouds = new List<Cloud>();
        private readonly Star[] _stars = new Star[Stars];
        private readonly GameData _data;
        private readonly BackdropArt _art;
        private readonly Func<TimeOfDay, Color> _skyTop;
        private readonly Func<TimeOfDay, Color> _ground;
        private readonly Func<TimeOfDay, float> _starStrength;

        public BackdropPainter(WorldScene scene, ArtParts parts, GameData data)
            : base(scene)
        {
            _parts = parts;
            _data = data;
            _art = parts.Art.Backdrop;
            _skyTop = time => data.LookOf(time).SkyTop;
            _ground = time => data.LookOf(time).Ground;
            _starStrength = time => data.LookOf(time).Stars;
            Seed();
        }

        private readonly struct Cloud
        {
            public Cloud(float x, float y, float speed, int index)
            {
                X = x;
                Y = y;
                Speed = speed;
                Index = index;
            }

            public float X { get; }
            public float Y { get; }
            public float Speed { get; }
            public int Index { get; }
        }

        private void Seed()
        {
            int seed = CloudSeed;
            for (int i = 0; i < TimeOfDayArt.CloudCount; i++)
            {
                float x = Seeded(ref seed) * 1900f;
                float y = 90f + Seeded(ref seed) * 200f;
                SkipToKeepCloudPositions(ref seed);
                SkipToKeepCloudPositions(ref seed);
                float speed = 0.04f + Seeded(ref seed) * 0.1f;
                SkipToKeepCloudPositions(ref seed);
                _clouds.Add(new Cloud(x, y, speed, i));
            }

            seed = StarSeed;
            for (int i = 0; i < Stars; i++)
            {
                float x = -400f + Seeded(ref seed) * StarsWidth;
                float y = -2000f + Seeded(ref seed) * 2500f;
                float radius = 0.4f + Seeded(ref seed) * 0.9f;
                float alpha = 0.35f + 0.65f * Seeded(ref seed);
                _stars[i] = new Star(x, y, radius, alpha);
            }
        }

        public Color GroundTint(SkyMix sky)
        {
            return sky.Mix(_ground);
        }

        private static float Seeded(ref int seed)
        {
            seed = (int)((long)seed * 16807 % 2147483647);
            return (seed - 1) / 2147483646f;
        }

        private static void SkipToKeepCloudPositions(ref int seed)
        {
            Seeded(ref seed);
        }

        private float LayerX(float parallax)
        {
            return Scene.BaseX - Scene.WalkDistance * parallax - Scene.CameraLag * (1f - parallax);
        }

        private void Tiles(Rect view, ArtLayer art, float x0, float period, DrawBand band, Color tint)
        {
            int from = Mathf.FloorToInt((view.xMin - x0) / period) - 1;
            int to = Mathf.FloorToInt((view.xMax - x0) / period) + 1;
            for (int k = from; k <= to; k++)
            {
                Canvas.Save();
                Canvas.Translate(x0 + k * period, 0f);
                _parts.Put(art, band, tint);
                Canvas.Restore();
            }
        }

        private void Strip(ArtLayer art, DrawBand band, Color tint)
        {
            _parts.StretchWidth(art, CoverLeft, 0f, CoverWidth, tint, band);
        }

        private void EachTime(SkyMix sky, bool fadesByWeight, Func<TimeOfDay, ArtLayer> art, Action<ArtLayer> draw)
        {
            for (int i = 0; i < TimeOfDays.Count; i++)
            {
                var time = (TimeOfDay)i;
                float alpha = fadesByWeight ? sky.Weight(time) : sky.LayerAlpha(time);
                ArtLayer layer = art(time);
                if (alpha <= 0f || layer == null)
                {
                    continue;
                }

                Canvas.Save();
                Canvas.Alpha *= alpha;
                draw(layer);
                Canvas.Restore();
            }
        }

        private Func<TimeOfDay, ArtLayer> ByTime(Func<TimeOfDayArt, ArtLayer> art)
        {
            return time => art(_data.LookOf(time).Art);
        }

        public void DrawSky(Rect view, SkyMix sky)
        {
            _parts.FillRect(CoverLeft, -SkyHeight, CoverWidth, SkyHeight, sky.Mix(_skyTop), DrawBand.Backdrop);
            EachTime(sky, false, ByTime(art => art.Sky), layer => Strip(layer, DrawBand.Backdrop, Color.white));
            DrawStars(view, sky);
            float sx = 880f + LayerX(0.02f);
            Canvas.Save();
            Canvas.Translate(sx, 230f);
            EachTime(sky, false, ByTime(art => art.Sun), layer => _parts.Put(layer, DrawBand.Backdrop));
            Canvas.Restore();
            foreach (Cloud cloud in _clouds)
            {
                float x = Mathf.Repeat(cloud.X + Scene.ElapsedFrames * cloud.Speed + LayerX(0.05f), 1900f) - 300f;
                Canvas.Save();
                Canvas.Translate(x, cloud.Y);
                EachTime(sky, false, ByTime(art => art.Clouds[cloud.Index]), layer => _parts.Put(layer, DrawBand.Backdrop));
                Canvas.Restore();
            }
        }

        private void DrawStars(Rect view, SkyMix sky)
        {
            float strength = sky.Mix(_starStrength);
            if (strength <= 0.01f)
            {
                return;
            }

            float shift = LayerX(0.02f);
            for (int i = 0; i < Stars; i++)
            {
                Star star = _stars[i];
                float k = Mathf.Clamp01((StarFadeY - star.Y) / StarFadeSpan) * strength * star.Alpha;
                if (k <= 0f || star.Y < view.yMin - 2f || star.Y > view.yMax + 2f)
                {
                    continue;
                }

                float x = view.xMin + Mathf.Repeat(star.X + shift - view.xMin, StarsWidth);
                if (x > view.xMax + 2f)
                {
                    continue;
                }

                _parts.Disc(x, star.Y, star.Radius, StarLight.WithAlpha(k), DrawBand.Backdrop);
            }
        }

        public void DrawFar(Rect view, SkyMix sky)
        {
            foreach (BackdropLayer far in _art.Far)
            {
                if (far.IsStrip)
                {
                    EachTime(sky, far.FadesByWeight, far.At, layer => Strip(layer, DrawBand.Backdrop, Color.white));
                }
                else
                {
                    float x0 = Mathf.Repeat(LayerX(far.Parallax), far.Period);
                    EachTime(sky, far.FadesByWeight, far.At, layer => Tiles(view, layer, x0, far.Period, DrawBand.Backdrop, Color.white));
                }
            }

            Color tint = GroundTint(sky);
            Canvas.Save();
            Canvas.Translate(Scene.WorldX, 0f);
            _parts.Put(_art.Hill, DrawBand.Backdrop, tint);
            Canvas.Restore();
            DrawProps(tint);
        }

        private void DrawProps(Color tint)
        {
            for (int i = 0; i < 40; i++)
            {
                float x = 1400f + i * 620f + Scene.WorldX;
                if (x < -150f || x > 1300f)
                {
                    continue;
                }

                Canvas.Save();
                Canvas.Translate(x, Ground);
                _parts.Put(_art.Props[i % _art.Props.Length], DrawBand.Backdrop, tint);
                Canvas.Restore();
            }
        }

        public void DrawGround(Rect view, SkyMix sky)
        {
            Color tint = GroundTint(sky);
            _parts.FillRect(CoverLeft, ScreenHeight, CoverWidth, UnderGroundHeight, UnderGround * tint, DrawBand.Ground);
            Strip(_art.Ground, DrawBand.Ground, tint);
            Tiles(view, _art.Rubble, Mathf.Repeat(Scene.WorldX, 1300f), 1300f, DrawBand.Ground, tint);
        }
    }
}
