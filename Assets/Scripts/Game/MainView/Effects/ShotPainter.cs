using System.Collections.Generic;
using Scrapwalker.Core.Combat;
using Scrapwalker.Core.Combat.Weapons;
using Scrapwalker.Game.Definitions;
using Scrapwalker.Game.Definitions.Flights;
using Scrapwalker.Game.Look;
using Scrapwalker.Game.Look.Art;
using Scrapwalker.Game.Look.Art.Parts;
using Scrapwalker.Game.Look.Art.Sets;
using Scrapwalker.Game.MainView.Compose;
using UnityEngine;

namespace Scrapwalker.Game.MainView.Effects
{
    public sealed class ShotPainter : WorldPainter
    {
        private const float LineArtLength = 100f;
        private const float BoltShapeFrames = 3f;
        private const int BoltShapes = 4;
        private const float FadeFrames = 8f;
        private const float BlastFrames = 24f;
        private const float StrikeHeight = 20f;
        private const float BulletLength = 14f;
        private const float BulletWidth = 1.5f;
        private static readonly SparkBurst StrikeSparks = new SparkBurst(5, Vector2.zero, new Vector2(0f, 2f), new Vector2(0f, -2f), 30f, 0f, Palette.Ember, 2f, 0f, 0.1f);


        private readonly EffectsPainter _effects;
        private readonly ArtParts _parts;
        private readonly List<Mark> _marks = new List<Mark>();
        private readonly List<Flight> _flights = new List<Flight>();
        private readonly HashSet<int> _flying = new HashSet<int>();
        private readonly Dictionary<ShotFlight, FlightArt> _arts = new Dictionary<ShotFlight, FlightArt>();
        private Depth _depth;

        private enum Depth
        {
            Glow,
            Core,
            Head,
        }

        public ShotPainter(WorldScene scene, EffectsPainter effects, ArtParts parts)
            : base(scene)
        {
            _effects = effects;
            _parts = parts;
        }

        private EffectArt Effects => _parts.Art.Effects;

        private struct Flight
        {
            public int ID;
            public FlightArt Art;
            public Vector2 Muzzle;
            public Vector2 Target;
            public Vector2 Position;
            public float Progress;
            public float ArcHeight;
            public float Frames;
        }

        public void Clear()
        {
            _marks.Clear();
            _flights.Clear();
        }

        public void AddStrikeLine(Vector2 from, Vector2 hit, ArtBolt bolt)
        {
            if (bolt != null)
            {
                AddMark(new BoltMark(from, hit, 10f, bolt.Color, bolt.Core));
            }
            else
            {
                AddMark(new StreakMark(from + new Vector2(0f, -StrikeHeight), hit, 6f, Palette.Red, 2f));
            }
        }

        private void AddMark(Mark mark)
        {
            _marks.Add(mark);
        }

        private void PutAt(ArtLayer layer, Vector2 at, float angle, Depth depth)
        {
            if (depth != _depth)
            {
                return;
            }

            _parts.PutAt(layer, at, angle, DrawBand.Effects);
        }

        private void Glow(float x, float y, float radiusX, float radiusY, Color tint, Depth depth)
        {
            if (depth != _depth)
            {
                return;
            }

            _parts.Glow(x, y, radiusX, radiusY, tint, DrawBand.Effects);
        }

        public void AddStrikeSparks(Vector2 hit)
        {
            _effects.Emit(StrikeSparks, hit);
        }

        private FlightArt ArtOf(ShotFlight flight)
        {
            if (!_arts.TryGetValue(flight, out FlightArt art))
            {
                art = flight.Show(new FlightArts());
                _arts[flight] = art;
            }

            return art;
        }

        public void AddShot(ShotFired shot, ShotLook look)
        {
            var facility = new FacilityRect(shot.FacilityRect);
            var muzzle = new Vector2(Scene.WallR + Scene.BaseX + shot.Origin * Scene.PixelsPerDistance, look.MuzzleY(facility.Top + Scene.BaseY, facility.PixelHeight));
            float targetX = Scene.FrontXOf(shot.LandingDistance) + Scene.RuinShift;
            _flights.Add(new Flight
            {
                ID = shot.ProjectileID,
                Art = ArtOf(look.Flight),
                Muzzle = muzzle,
                Target = new Vector2(targetX, WorldScene.Ground - look.HitHeight),
                Position = muzzle,
                ArcHeight = look.Flight.ArcFor(Mathf.Abs(targetX - muzzle.x)),
                Frames = Mathf.Max(1f, shot.FlightSeconds * WorldScene.FramesPerSecond),
            });
            _effects.Emit(look.Muzzle, muzzle);
        }

        public void Land(ShotLanded landing, ShotLook look)
        {
            bool isKnown = TryTake(landing.ProjectileID, out Flight flight);
            Vector2 hit = isKnown ? flight.Target : new Vector2(Scene.FrontXOf(landing.Distance) + Scene.RuinShift, WorldScene.Ground);
            float spread = ArtOf(look.Flight).Land(this, isKnown ? flight.Muzzle : (Vector2?)null, hit, landing);
            _effects.Emit(look.Hit.Spreading(new Vector2(spread, 0f)), hit);
        }

        private bool TryTake(int projectileID, out Flight flight)
        {
            int index = _flights.FindIndex(candidate => candidate.ID == projectileID);
            if (index < 0)
            {
                flight = default;
                return false;
            }

            flight = _flights[index];
            _flights.RemoveAt(index);
            return true;
        }

        public void RemoveVanished(IReadOnlyList<Projectile> projectiles)
        {
            _flying.Clear();
            foreach (Projectile projectile in projectiles)
            {
                _flying.Add(projectile.ID);
            }

            _flights.RemoveAll(flight => !_flying.Contains(flight.ID));
        }

        public void Step(float deltaFrames)
        {
            for (int i = 0; i < _flights.Count; i++)
            {
                Flight flight = _flights[i];
                flight.Progress = Mathf.Min(1f, flight.Progress + deltaFrames / flight.Frames);
                flight.Position = Vector2.Lerp(flight.Muzzle, flight.Target, flight.Progress) + new Vector2(0f, -Mathf.Sin(flight.Progress * Mathf.PI) * flight.ArcHeight);
                _flights[i] = flight;
            }

            _marks.RemoveAll(mark => mark.Age(deltaFrames));
        }

        public void Draw()
        {
            foreach (Depth depth in System.Enum.GetValues(typeof(Depth)))
            {
                _depth = depth;
                DrawAtDepth();
            }
        }

        private void Between(ArtLayer layer, ArtFrame frame, Vector2 from, Vector2 to, float thickness, Color tint, Depth depth)
        {
            if (depth == _depth)
            {
                _parts.PutBetween(layer, frame, from, to, LineArtLength, thickness, DrawBand.Effects, tint);
            }
        }

        private void DrawAtDepth()
        {
            foreach (Mark mark in _marks)
            {
                mark.Draw(this);
            }

            foreach (Flight flight in _flights)
            {
                flight.Art.Draw(this, flight);
            }
        }

        private void DrawBullet(Vector2 from, Vector2 to, Color color)
        {
            ArtLayer core = Effects.ShotLine.Core;
            Between(core, core.First, from, to, BulletWidth / 2f, color, Depth.Core);
        }

        private void DrawStreak(Vector2 from, Vector2 to, Color color, float width, float alpha)
        {
            Color tint = color.WithAlpha(alpha);
            ArtLayer glow = Effects.ShotLine.Glow;
            ArtLayer core = Effects.ShotLine.Core;
            Between(glow, glow.First, from, to, width / 2f, tint, Depth.Glow);
            Between(core, core.First, from, to, width / 2f, tint, Depth.Core);
        }

        private void DrawBolt(Vector2 from, Vector2 to, Color glowColor, Color coreColor, float alpha)
        {
            int shape = Mathf.FloorToInt(Scene.ElapsedFrames / BoltShapeFrames) % BoltShapes;
            ArtLayer glow = Effects.Bolt.Glow;
            ArtLayer core = Effects.Bolt.Core;
            Between(glow, glow.ByValue(shape), from, to, 1f, glowColor.WithAlpha(alpha), Depth.Glow);
            Between(core, core.ByValue(shape), from, to, 1f, new Color(1f, 1f, 1f, alpha), Depth.Core);
        }

        private sealed class FlightArts : IShotFlightView<FlightArt>
        {
            public FlightArt Tracer(TracerFlight tracer) => new TracerArt(tracer.Color);

            public FlightArt Bolt(BoltFlight bolt) => new BoltArt(bolt.LineColor, bolt.LineCore);

            public FlightArt Shell(ShellFlight shell) => new ShellArt();

            public FlightArt Lob(LobFlight lob) => new LobArt();
        }

        private abstract class FlightArt
        {
            public abstract void Draw(ShotPainter painter, in Flight flight);

            public virtual float Land(ShotPainter painter, Vector2? muzzle, Vector2 hit, ShotLanded landing)
            {
                return 0f;
            }
        }

        private sealed class TracerArt : FlightArt
        {
            private readonly Color _color;

            public TracerArt(Color color)
            {
                _color = color;
            }

            public override void Draw(ShotPainter painter, in Flight flight)
            {
                Vector2 direction = (flight.Target - flight.Muzzle).normalized;
                Vector2 tail = flight.Position - direction * Mathf.Min(BulletLength, Vector2.Distance(flight.Muzzle, flight.Position));
                painter.DrawBullet(tail, flight.Position, _color);
            }
        }

        private sealed class BoltArt : FlightArt
        {
            private readonly Color _lineColor;
            private readonly Color _lineCore;

            public BoltArt(Color lineColor, Color lineCore)
            {
                _lineColor = lineColor;
                _lineCore = lineCore;
            }

            public override void Draw(ShotPainter painter, in Flight flight)
            {
                painter.DrawBolt(flight.Muzzle, flight.Position, _lineColor, _lineCore, 1f);
            }

            public override float Land(ShotPainter painter, Vector2? muzzle, Vector2 hit, ShotLanded landing)
            {
                if (muzzle.HasValue)
                {
                    painter.AddMark(new BoltMark(muzzle.Value, hit, 6f, _lineColor, _lineCore));
                }

                return 0f;
            }
        }

        private sealed class ShellArt : FlightArt
        {
            public override void Draw(ShotPainter painter, in Flight flight)
            {
                float angle = Mathf.Atan2(flight.Target.y - flight.Muzzle.y - Mathf.Cos(flight.Progress * Mathf.PI) * Mathf.PI * flight.ArcHeight, flight.Target.x - flight.Muzzle.x);
                painter.PutAt(painter.Effects.ShellTail, flight.Position, angle, Depth.Core);
                painter.PutAt(painter.Effects.ShellBody, flight.Position, angle, Depth.Head);
            }

            public override float Land(ShotPainter painter, Vector2? muzzle, Vector2 hit, ShotLanded landing)
            {
                WorldScene scene = painter.Scene;
                float half = Mathf.Abs(scene.FrontXOf(landing.Distance + landing.Radius) - scene.FrontXOf(landing.Distance));
                painter.AddMark(new BlastMark(hit, Mathf.Max(12f, half)));
                return half;
            }
        }

        private sealed class LobArt : FlightArt
        {
            public override void Draw(ShotPainter painter, in Flight flight)
            {
                painter.PutAt(painter.Effects.Lob, flight.Position, flight.Progress * 12f, Depth.Head);
            }
        }

        private abstract class Mark
        {
            private bool _isFresh = true;

            protected Mark(float lifeFrames)
            {
                LifeFrames = lifeFrames;
            }

            protected float LifeFrames { get; private set; }

            protected float Opacity => Mathf.Clamp01(LifeFrames / FadeFrames);

            public bool Age(float deltaFrames)
            {
                if (_isFresh)
                {
                    _isFresh = false;
                    return false;
                }

                LifeFrames -= deltaFrames;
                return LifeFrames <= 0f;
            }

            public abstract void Draw(ShotPainter painter);
        }

        private sealed class StreakMark : Mark
        {
            private readonly Vector2 _from;
            private readonly Vector2 _to;
            private readonly Color _color;
            private readonly float _width;

            public StreakMark(Vector2 from, Vector2 to, float lifeFrames, Color color, float width)
                : base(lifeFrames)
            {
                _from = from;
                _to = to;
                _color = color;
                _width = width;
            }

            public override void Draw(ShotPainter painter)
            {
                painter.DrawStreak(_from, _to, _color, _width, Opacity);
            }
        }

        private sealed class BoltMark : Mark
        {
            private readonly Vector2 _from;
            private readonly Vector2 _to;
            private readonly Color _glow;
            private readonly Color _core;

            public BoltMark(Vector2 from, Vector2 to, float lifeFrames, Color glow, Color core)
                : base(lifeFrames)
            {
                _from = from;
                _to = to;
                _glow = glow;
                _core = core;
            }

            public override void Draw(ShotPainter painter)
            {
                painter.DrawBolt(_from, _to, _glow, _core, Opacity);
            }
        }

        private sealed class BlastMark : Mark
        {
            private readonly Vector2 _at;
            private readonly float _halfWidth;

            public BlastMark(Vector2 at, float halfWidth)
                : base(BlastFrames)
            {
                _at = at;
                _halfWidth = halfWidth;
            }

            public override void Draw(ShotPainter painter)
            {
                float spread = 1f - LifeFrames / BlastFrames;
                Color light = Palette.BlastLight;
                light.a *= 1f - spread;
                painter.Glow(_at.x, _at.y - 6f, _halfWidth * (0.6f + spread * 0.6f), 10f + spread * 14f, light, Depth.Core);
            }
        }
    }
}
