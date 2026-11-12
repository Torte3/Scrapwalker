using System.Collections.Generic;
using Scrapwalker.Core.Items;
using Scrapwalker.Game.Definitions;
using Scrapwalker.Game.Look;
using Scrapwalker.Game.Look.Art;
using Scrapwalker.Game.MainView.Outpost.Limbs;
using UnityEngine;

namespace Scrapwalker.Game.MainView.Effects
{
    public sealed class DropPainter : WorldPainter
    {
        private const float Gravity = 0.22f;
        private const float RestAboveGround = 3f;
        private const float SwallowDistance = 10f;

        private readonly ArtParts _parts;
        private readonly GameData _data;
        private readonly List<Drop> _drops = new List<Drop>();
        private bool _isPulling;

        public DropPainter(WorldScene scene, ArtParts parts, GameData data)
            : base(scene)
        {
            _parts = parts;
            _data = data;
        }

        private sealed class Drop
        {
            public Vector2 Position;
            public Vector2 Velocity;
            public float Angle;
            public float Spin;
            public float Pull;
            public bool IsLanded;
            public ArtLayer Art;
        }

        public void Clear()
        {
            _drops.Clear();
        }

        public void Spawn(float x, float y, IReadOnlyList<MaterialAmount> drops)
        {
            foreach (MaterialAmount drop in drops)
            {
                ArtLayer art = _data.Asset(drop.Type).DropArt;
                for (int i = 0; i < drop.Count; i++)
                {
                    float px = x + (Scene.Random() - 0.5f) * 26f;
                    float vy = -1.4f - Scene.Random() * 1.6f;
                    float vx = (Scene.Random() - 0.5f) * 1.2f;
                    _drops.Add(new Drop
                    {
                        Position = new Vector2(px, y - 6f),
                        Velocity = new Vector2(vx, vy),
                        Angle = Scene.Random() * 6.3f,
                        Spin = (Scene.Random() - 0.5f) * 0.3f,
                        Art = art,
                    });
                }
            }
        }

        private Vector2 IntakePoint(int intakeRow)
        {
            return new Vector2(Scene.WallR + ArmMotion.DropGap * 0.5f + Scene.BaseX, ArmMotion.DropY(intakeRow) + Scene.BaseY);
        }

        public void Step(float deltaFrames, int intakeRow)
        {
            float scroll = Scene.ScrollPixels;
            Vector2 intake = IntakePoint(intakeRow);
            _isPulling = false;
            for (int i = _drops.Count - 1; i >= 0; i--)
            {
                Drop drop = _drops[i];
                drop.Position.x += scroll;
                if (!drop.IsLanded)
                {
                    Fall(drop, deltaFrames);
                }
                else
                {
                    _isPulling = true;
                    if (PullIn(drop, intake, deltaFrames))
                    {
                        _drops.RemoveAt(i);
                    }
                }
            }
        }

        private static void Fall(Drop drop, float deltaFrames)
        {
            drop.Velocity.y += Gravity * deltaFrames;
            drop.Position += drop.Velocity * deltaFrames;
            drop.Angle += drop.Spin * deltaFrames;
            float floor = WorldScene.Ground - RestAboveGround;
            if (drop.Position.y < floor)
            {
                return;
            }

            drop.Position.y = floor;
            if (Mathf.Abs(drop.Velocity.y) > 1.2f)
            {
                drop.Velocity.y *= -0.35f;
                drop.Velocity.x *= 0.5f;
                drop.Spin *= 0.5f;
            }
            else
            {
                drop.IsLanded = true;
                drop.Spin = 0f;
            }
        }

        private static bool PullIn(Drop drop, Vector2 intake, float deltaFrames)
        {
            drop.Pull = Mathf.Min(1f, drop.Pull + 0.012f * deltaFrames);
            float speed = (0.6f + drop.Pull * 2.6f) * deltaFrames;
            Vector2 toIntake = intake - drop.Position;
            float distance = toIntake.magnitude;
            if (distance < 0.001f)
            {
                distance = 1f;
            }

            Vector2 direction = toIntake / distance;
            drop.Position += new Vector2(direction.x * speed, direction.y * speed * (distance < 40f ? 1f : 0.25f));
            drop.Angle += 0.12f * deltaFrames;
            return distance < SwallowDistance;
        }

        public void DrawBelow(int intakeRow)
        {
            if (_isPulling)
            {
                Vector2 intake = IntakePoint(intakeRow);
                _parts.Glow(intake.x, intake.y, 18f, 18f, Palette.Amber.WithAlpha(0.3f), DrawBand.Effects);
            }

            foreach (Drop drop in _drops)
            {
                _parts.PutAt(drop.Art, drop.Position, drop.Angle, DrawBand.Effects);
            }
        }

        public void DrawAbove(int intakeRow)
        {
            if (_isPulling)
            {
                DrawIntake(IntakePoint(intakeRow));
            }

            foreach (Drop drop in _drops)
            {
                if (drop.IsLanded && drop.Pull > 0.05f)
                {
                    _parts.FillRect(drop.Position.x + 6f, drop.Position.y + 3f, 11f, 2f, Palette.DropShadow, DrawBand.Effects);
                }
            }
        }

        private void DrawIntake(Vector2 intake)
        {
            ArtLayer arc = _parts.Art.Effects.PullArc;
            Color ring = Palette.Amber.WithAlpha(arc.Alpha(Scene.ElapsedFrames));
            for (int i = 1; i <= 3; i++)
            {
                float radius = 16f * i + Scene.ElapsedFrames % 30f;
                Canvas.Save();
                Canvas.Translate(intake.x, intake.y);
                _parts.Put(arc, arc.ByValue(radius), DrawBand.Effects, ring);
                Canvas.Restore();
            }
        }
    }
}
