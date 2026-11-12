using Scrapwalker.Game.Look;
using Scrapwalker.Game.Look.Art;
using Scrapwalker.Game.MainView.Effects;
using UnityEngine;
using static Scrapwalker.Game.Look.Pen;
using static Scrapwalker.Game.MainView.WorldScene;

namespace Scrapwalker.Game.MainView.Launch
{
    public sealed class LaunchPainter : WorldPainter
    {
        private const float WheelNear = 150f;
        private const float WheelFar = 60f;
        private const float WheelTravel = 36f;
        private const float FireFlash = 0.8f;
        private const float FlashFade = 0.04f;
        private const int SpeedLineCount = 14;
        private const float FlameGlowSpread = 0.9f;
        private const float FlameGlowAlpha = 0.25f;
        private const float ShakeFadeHeight = 40f;

        private static readonly Rect FlashCover = new Rect(-20000f, -40000f, 40000f, 60000f);
        private static readonly PuffBurst IgnitionSmoke = new PuffBurst(30, new Vector2(120f, 0f), new Vector2(-2f, -10f), new Vector2(0f, -3f), 60f, 8f, 10f, PuffKind.Smoke);
        private static readonly SparkLook ScrapeLook = new SparkLook(Palette.LaunchScrape, 2f, 0.2f);

        private readonly ArtParts _parts;
        private readonly EffectsPainter _effects;
        private readonly LaunchMotion _motion;
        private readonly BoosterMotion _boosterMotion;
        private readonly BoosterPainter _booster;
        private float _lastFramesSinceLaunch = float.NegativeInfinity;

        public LaunchPainter(WorldScene scene, ArtParts parts, EffectsPainter effects, LaunchMotion motion, BoosterMotion boosterMotion, BoosterPainter booster)
            : base(scene)
        {
            _parts = parts;
            _effects = effects;
            _motion = motion;
            _boosterMotion = boosterMotion;
            _booster = booster;
        }

        public float Flash { get; private set; }

        public void Clear()
        {
            Flash = 0f;
            _lastFramesSinceLaunch = float.NegativeInfinity;
        }

        public void Step(float deltaFrames)
        {
            Flash = Mathf.Max(0f, Flash - FlashFade * deltaFrames);
            if (!_motion.IsActive)
            {
                return;
            }

            float frames = _motion.FramesSinceLaunch;
            Vector2 nozzle = _boosterMotion.Nozzle;
            if (Crossed(-LaunchMotion.SettleFrames, frames))
            {
                Scene.Quake(6f);
                _effects.Burst(nozzle.x + 14f, nozzle.y);
            }

            if (Crossed(LaunchMotion.FireAt, frames))
            {
                Ignite(nozzle);
            }

            _lastFramesSinceLaunch = frames;
            float build = _motion.Build;
            if (build > 0f)
            {
                BuildUp(nozzle, build, deltaFrames);
            }

            if (_motion.IsFired)
            {
                Scene.Quake((3f + 6f * _motion.Ignition) * GroundShake());
                Scrape();
            }
        }

        private float GroundShake()
        {
            return 1f - SmoothRamp(-_motion.Height, 0f, ShakeFadeHeight);
        }

        private bool Crossed(float at, float frames)
        {
            return _lastFramesSinceLaunch < at && frames >= at;
        }

        private void BuildUp(Vector2 nozzle, float build, float deltaFrames)
        {
            int motes = Mathf.RoundToInt((1 + Mathf.Floor(build * 4f)) * deltaFrames);
            for (int i = 0; i < motes; i++)
            {
                _booster.Mote(nozzle.x, nozzle.y, 120f + Scene.Random() * 160f, 22f);
            }

            if (Scene.IsEvery(3))
            {
                _effects.Puff(nozzle.x + 30f + Scene.Random() * 40f, Ground - 4f, -0.5f - Scene.Random() * 2f * build, -0.5f - Scene.Random() * 1.5f * build, 50f, 5f + 6f * build, PuffKind.Smoke);
            }

            Scene.Quake(1f + 7f * build * build);
        }

        private void Ignite(Vector2 nozzle)
        {
            Flash = FireFlash;
            Scene.Quake(22f);
            _effects.Emit(IgnitionSmoke, new Vector2(nozzle.x, Ground - 4f));
        }

        private void Scrape()
        {
            if (_motion.Height <= -12f)
            {
                return;
            }

            float speed = _motion.Speed;
            if (Scene.IsEvery(2))
            {
                for (int i = 0; i < 3; i++)
                {
                    _effects.Puff(Scene.WallL - 20f, Ground - 4f, -speed * (0.3f + Scene.Random() * 0.3f), -Scene.Random() * 2f, 40f, 6f + Scene.Random() * 8f, PuffKind.Smoke);
                }
            }

            _effects.Spark(Scene.WallL + 10f, Ground - 2f, -speed * 0.5f, -Scene.Random() * 3f, 14f, ScrapeLook);
        }

        public void DrawWheels()
        {
            float k = _motion.IsActive ? _motion.WheelOut : 0f;
            if (k <= 0f)
            {
                return;
            }

            float right = HullLeft + Scene.HullWidth;
            float spin = _motion.Distance / 18f;
            float y = Ground - 18f + WheelTravel * k;
            float[] wheels = { right - WheelNear, right - WheelFar };
            foreach (float x in wheels)
            {
                _parts.FillRect(x - 4f, Ground - 24f, 8f, y - (Ground - 24f), Palette.LaunchStrut, DrawBand.Legs);
            }

            foreach (float x in wheels)
            {
                PutWheel(_parts.Art.Takeoff.Tire, x, y, 0f);
            }

            foreach (float x in wheels)
            {
                PutWheel(_parts.Art.Takeoff.Spokes, x, y, spin);
            }
        }

        private void PutWheel(ArtLayer part, float x, float y, float angle)
        {
            Canvas.Save();
            Canvas.Translate(x, y);
            Canvas.Rotate(angle);
            _parts.Put(part, DrawBand.Legs);
            Canvas.Restore();
        }

        public void DrawFire()
        {
            if (!_motion.IsActive)
            {
                return;
            }

            Vector2 nozzle = _boosterMotion.Nozzle;
            float build = _motion.Build;
            if (build > 0f)
            {
                float r = 60f + 140f * build;
                _parts.Glow(nozzle.x, nozzle.y, r, r, Palette.LaunchBuild.WithAlpha(0.25f + 0.45f * build), DrawBand.Effects);
                Flame(nozzle, 30f + 40f * build, BoosterMotion.Height * 0.35f * build + 10f, 0.35f + 0.3f * build);
            }

            if (_motion.IsFired)
            {
                Flame(nozzle, 200f + _motion.Speed * 2.5f, BoosterMotion.Height * 0.8f, _motion.Ignition * 1.5f);
            }

            if (_motion.Speed > 30f)
            {
                SpeedLines();
            }
        }

        private void Flame(Vector2 at, float baseLength, float width, float k)
        {
            float length = baseLength * (0.86f + 0.14f * Mathf.Sin(Scene.ElapsedFrames * 1.3f + at.x)) * k;
            if (length < 1f)
            {
                return;
            }

            Canvas.Save();
            Canvas.Translate(at.x, at.y);
            Canvas.Rotate(Mathf.PI);
            Canvas.Scale(length / 100f, width / 40f);
            _parts.Put(_parts.Art.Takeoff.Flame, DrawBand.Effects);
            Canvas.Restore();
            float glow = width * FlameGlowSpread * k;
            _parts.Glow(at.x, at.y, glow, glow, Palette.LaunchFlame.WithAlpha(FlameGlowAlpha * k), DrawBand.Effects);
        }

        private void SpeedLines()
        {
            float speed = _motion.Speed;
            Color color = Palette.SpeedLine.WithAlpha(Mathf.Min(0.5f, (speed - 30f) / 120f));
            float step = Mathf.Floor(Scene.ElapsedFrames / 3f);
            for (int i = 0; i < SpeedLineCount; i++)
            {
                float y = Scene.HullTop - 120f + Hash(i * 3.1f) * 520f;
                float x = Scene.WallL - 200f - Hash(i * 7.7f + step) * 700f;
                float length = 80f + speed * 2f;
                _parts.FillRect(x - length, y - 0.75f, length, 1.5f, color, DrawBand.Effects);
            }
        }

        public void DrawFlash()
        {
            if (Flash > 0f)
            {
                _parts.FillRect(FlashCover.x, FlashCover.y, FlashCover.width, FlashCover.height, Palette.LaunchFlash.WithAlpha(Flash), DrawBand.Effects);
            }
        }
    }
}
