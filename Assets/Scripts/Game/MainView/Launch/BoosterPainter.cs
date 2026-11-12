using Scrapwalker.Game.MainView.Compose;
using Scrapwalker.Game.MainView.Effects;
using UnityEngine;
using static Scrapwalker.Game.Look.Palette;
using static Scrapwalker.Game.Look.Pen;
using static Scrapwalker.Game.MainView.WorldScene;

namespace Scrapwalker.Game.MainView.Launch
{
    public sealed class BoosterPainter : WorldPainter
    {
        private const int Segments = 11;
        private const float DoorHeight = (BoosterMotion.Height + 16f) / 4f;
        private const float StartPulse = 0.8f;
        private const float StartLights = 2f;
        private const float StartGlow = 0.18f;
        private const float ChaseFrames = 70f;

        private static readonly SparkLook MoteLook = new SparkLook(BoosterMote, 3f, 0f);
        private static readonly SparkLook Spark = new SparkLook(BoosterSpark, 2f, 0.15f);
        private static readonly PuffBurst ChargeSteam = new PuffBurst(1, new Vector2(0f, BoosterMotion.Height), new Vector2(-0.6f, -1.8f), new Vector2(-0.3f, -0.9f), 45f, 5f, 0f, PuffKind.Steam);
        private static readonly PuffBurst ChargeSmoke = new PuffBurst(1, new Vector2(40f, 0f), new Vector2(-0.3f, -1.3f), new Vector2(-0.3f, -0.9f), 40f, 4f, 0f, PuffKind.Smoke);
        private static readonly PuffBurst DoorSteam = new PuffBurst(1, new Vector2(0f, BoosterMotion.Height), new Vector2(-1f, -3f), new Vector2(-0.75f, 0.75f), 40f, 6f, 0f, PuffKind.Steam);
        private readonly float[] _lit = new float[Segments];

        private readonly ArtParts _parts;
        private readonly EffectsPainter _effects;
        private readonly BoosterMotion _motion;

        public BoosterPainter(WorldScene scene, BoosterMotion motion, ArtParts parts, EffectsPainter effects)
            : base(scene)
        {
            _motion = motion;
            _parts = parts;
            _effects = effects;
        }

        public void Step(in LaunchVisual launch, float deltaFrames)
        {
            _motion.Step(launch, deltaFrames);
            if (!launch.IsLaunching && launch.IsCharging)
            {
                ChargeAmbience(deltaFrames);
            }
        }

        private void ChargeAmbience(float deltaFrames)
        {
            float c = _motion.Charge;
            float y = _motion.CenterY;
            float nozzleX = Scene.WallL - BoosterMotion.OutPixels + BoosterMotion.NozzleTip;
            if (c > 0.5f && Scene.IsEvery(Mathf.Max(4, Mathf.RoundToInt(30f - 40f * (c - 0.5f)))))
            {
                _effects.Emit(ChargeSteam, new Vector2(Scene.WallL - 4f, y));
            }

            if (c > 0.7f && Scene.Random() < (c - 0.7f) * 2.5f * deltaFrames)
            {
                Mote(nozzleX, y, 110f + Scene.Random() * 120f, 26f);
            }

            if (c > 0.75f)
            {
                Scene.Quake((c - 0.75f) * 8f);
                if (Scene.IsEvery(6))
                {
                    _effects.Emit(ChargeSmoke, new Vector2(nozzleX + 40f, Ground - 4f));
                }
            }
        }

        public void Mote(float x, float y, float r, float life)
        {
            float a = Scene.Random() * Mathf.PI * 2f;
            float dx = Mathf.Cos(a) * r;
            float dy = Mathf.Sin(a) * r * 0.8f;
            _effects.Spark(x + dx, y + dy, -dx / life, -dy / life, life, MoteLook);
        }

        private struct Doors
        {
            public float Top;
            public float Open;
            public float Push;
            public float Slide;
            public float Flare;
        }

        public void Draw()
        {
            if (_motion.Door <= 0f && _motion.Out <= 0f)
            {
                return;
            }

            Doors doors = DoorsNow();
            bool isOut = _motion.Out > 0f;
            if (isOut)
            {
                DrawBooster();
            }

            for (int j = 0; j < 2; j++)
            {
                for (int side = -1; side <= 1; side += 2)
                {
                    DrawDoorArt(doors, side, j);
                }
            }

            if (isOut)
            {
                DrawArcs();
            }

            if (doors.Slide > 0f)
            {
                for (int j = 0; j < 2; j++)
                {
                    for (int side = -1; side <= 1; side += 2)
                    {
                        DrawDoorLight(doors, side, j);
                    }
                }
            }

            DrawSeams(doors);
        }

        private Color BandColor => Color.LerpUnclamped(BoosterLight, BoosterCharged, _motion.Charge);

        private void DrawBooster()
        {
            float x = _motion.BodyX;
            float y = _motion.CenterY;
            float c = _motion.Charge;
            float height = BoosterMotion.Height;
            bool isFull = c >= 1f;
            float pulse = 0.65f + 0.35f * Mathf.Sin(Scene.ElapsedFrames * (0.06f + 0.3f * c)) * Mathf.Max(StartPulse, Mathf.Min(1f, c * 1.5f));
            float lights = Mathf.Max(StartLights, c * Segments);
            float chase = isFull ? -Segments : Mathf.Repeat(Scene.ElapsedFrames / ChaseFrames, 1.4f) * Segments;
            float chaseLight = 0.9f * (1f - c);
            Color band = BandColor;
            Canvas.Save();
            Canvas.Translate(x, y);
            _parts.Put(_parts.Art.Takeoff.Booster, DrawBand.Legs);
            Canvas.Restore();

            for (int i = 0; i < Segments; i++)
            {
                float lit = Mathf.Max(Mathf.Clamp01(lights - i) * pulse, Mathf.Clamp01(1f - Mathf.Abs(chase - i) * 0.5f) * chaseLight);
                _parts.FillRect(x + 15f, SegmentY(y, i), 6f, 9f, band.WithAlpha(0.85f * lit), DrawBand.Legs);
                _lit[i] = lit;
            }

            float hot = _motion.Out * (isFull ? 1f : 0.35f);
            float exhaustX = x + BoosterMotion.NozzleTip;
            _parts.FillRect(exhaustX + 3f, y - height * 0.36f, 6f, height * 0.72f, (isFull ? ExhaustFull : ExhaustWarm).WithAlpha(0.5f * hot), DrawBand.Legs);
            for (int i = 0; i < Segments; i++)
            {
                if (_lit[i] > 0.5f)
                {
                    _parts.Glow(x + 18f, SegmentY(y, i) + 4f, 16f, 16f, BoosterLight.WithAlpha(0.35f * _lit[i]), DrawBand.Legs);
                }
            }

            float glow = _motion.Out * Mathf.Max(StartGlow, 0.08f + 0.2f * c);
            float r = 60f + 80f * c;
            _parts.Glow(x + 10f, y, r, r, band.WithAlpha(glow * pulse), DrawBand.Legs);
        }

        private static float SegmentY(float y, int i)
        {
            return y + BoosterMotion.Height * 0.5f - 34f - i * 14f;
        }

        private void DrawArcs()
        {
            float c = _motion.Charge;
            int every = c >= 1f ? 11 : Mathf.Max(11, Mathf.RoundToInt(60f - 90f * (c - 0.5f)));
            if (c > 0.5f && Mathf.Repeat(Scene.ElapsedFrames, every) < 3f)
            {
                Arc(_motion.BodyX + 18f, _motion.CenterY - BoosterMotion.Height * 0.5f - 6f);
            }
        }

        private void Arc(float x, float y)
        {
            float px = x;
            float py = y;
            for (int i = 1; i <= 5; i++)
            {
                float nx = x - i * 8f + (Scene.Random() - 0.5f) * 12f;
                float ny = y - i * 6f + (Scene.Random() - 0.5f) * 8f;
                Segment(px, py, nx, ny, 1.8f, BoosterArc);
                px = nx;
                py = ny;
            }
        }

        private void Segment(float x1, float y1, float x2, float y2, float width, Color color)
        {
            Canvas.Save();
            Canvas.Translate(x1, y1);
            Canvas.Rotate(Direction(new Vector2(x1, y1), new Vector2(x2, y2)));
            _parts.FillRect(0f, -width * 0.5f, Mathf.Sqrt((x2 - x1) * (x2 - x1) + (y2 - y1) * (y2 - y1)), width, color, DrawBand.Legs);
            Canvas.Restore();
        }

        private Doors DoorsNow()
        {
            float door = _motion.Door;
            return new Doors
            {
                Top = _motion.CenterY - BoosterMotion.Height * 0.5f - 8f,
                Open = BoosterMotion.Height + 16f,
                Push = 9f * SmoothRamp(door, 0f, 0.3f),
                Slide = SmoothRamp(door, 0.3f, 0.5f),
                Flare = SmoothRamp(door, 0.8f, 0.2f),
            };
        }

        private void DrawSeams(in Doors doors)
        {
            float door = _motion.Door;
            if (door > 0f && door < 0.4f)
            {
                Color seam = BoosterSeam.WithAlpha(0.85f * Mathf.Sin(Mathf.Clamp01(door / 0.4f) * Mathf.PI));
                for (int i = 1; i < 4; i++)
                {
                    _parts.FillRect(Scene.WallL - 4f, doors.Top + DoorHeight * i - 1f, Armor + 4f, 2f, seam, DrawBand.Legs);
                }

                _parts.FillRect(Scene.WallL - 4f, doors.Top, 2f, doors.Open, seam, DrawBand.Legs);
            }

            if (door > 0.3f && door < 0.85f && Scene.IsEvery(3))
            {
                float y = _motion.CenterY;
                _effects.Emit(DoorSteam, new Vector2(Scene.WallL - 12f, y));
                _effects.Spark(Scene.WallL - 6f, doors.Top + (Scene.Random() < 0.5f ? 0f : doors.Open), -Scene.Random() * 3f, (Scene.Random() - 0.5f) * 3f, 16f, Spark);
            }
        }

        private void DrawDoorArt(in Doors doors, int side, int j)
        {
            DoorAt(doors, side, j, out float x0, out float y0);
            Canvas.Save();
            if (side < 0)
            {
                Canvas.Translate(x0, y0);
            }
            else
            {
                Canvas.Translate(x0, y0 + DoorHeight);
                Canvas.Scale(1f, -1f);
            }

            _parts.Put(j == 0 ? _parts.Art.Takeoff.DoorEdge : _parts.Art.Takeoff.DoorMiddle, DrawBand.Legs);
            Canvas.Restore();
            Canvas.Restore();
        }

        private void DrawDoorLight(in Doors doors, int side, int j)
        {
            DoorAt(doors, side, j, out float x0, out float y0);
            _parts.FillRect(x0 - 4f, y0 + 4f, 2f, DoorHeight - 8f, BoosterLight.WithAlpha(0.45f * doors.Slide), DrawBand.Legs);
            Canvas.Restore();
        }

        private void DoorAt(in Doors doors, int side, int j, out float x0, out float y0)
        {
            float top = side < 0 ? doors.Top + j * DoorHeight : doors.Top + doors.Open - (j + 1) * DoorHeight;
            float dy = side * (DoorHeight * j + DoorHeight * 0.9f) * doors.Slide;
            x0 = Scene.WallL - doors.Push - j * 6f * doors.Slide;
            y0 = top + dy;
            float hingeY = side < 0 ? y0 + DoorHeight : y0;
            Canvas.Save();
            Canvas.RotateAbout(new Vector2(x0, hingeY), side * -0.35f * doors.Flare);
        }
    }
}
