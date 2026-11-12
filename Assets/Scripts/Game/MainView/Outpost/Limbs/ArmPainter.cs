using System.Collections.Generic;
using Scrapwalker.Core.Root;
using Scrapwalker.Game.Definitions;
using Scrapwalker.Core.Items;
using Scrapwalker.Game.Look;
using Scrapwalker.Game.Look.Art;
using Scrapwalker.Game.Look.Art.Sets;
using Scrapwalker.Game.MainView.Compose;
using Scrapwalker.Game.MainView.Effects;
using UnityEngine;
using static Scrapwalker.Game.MainView.WorldScene;

namespace Scrapwalker.Game.MainView.Outpost.Limbs
{
    public sealed class ArmPainter : WorldPainter
    {
        private static readonly Color[] PileQualityColors = { Palette.Quality(Quality.Plain), Palette.Quality(Quality.High), Palette.Quality(Quality.Good) };
        private static readonly float[][] PileOffsets = { new[] { -9.6f, -7.2f }, new[] { 7.2f, -8.8f }, new[] { 0f, 4f } };
        private static readonly SparkBurst PulledDebris = new SparkBurst(14, new Vector2(0f, 50f), new Vector2(0f, -1.5f), new Vector2(0f, 1f), 60f, 0f, Palette.PulledDebris, 2f, 3f, 0.12f);
        private const float ClawClosed = 0.3f;
        private const float ClawOpenRange = 0.55f;
        private const float InboundPixelsPerFrame = 1.3f;
        private const float InboundFadePixels = 14f;

        private static readonly SparkBurst DigSparks = new SparkBurst(3, new Vector2(0f, 18f), new Vector2(0f, -3f), new Vector2(0f, -2.5f), 16f, 0f, Palette.Spark, 2f, 0f, 0.15f);
        private static readonly SparkBurst DigDebris = new SparkBurst(1, new Vector2(22f, 16f), new Vector2(-0.4f, -1.3f), new Vector2(-0.3f, -1f), 28f, 0f, Palette.Debris, 2f, 2f, 0.04f);
        private static readonly PuffBurst DigSmoke = new PuffBurst(1, new Vector2(34f, 24f), new Vector2(-0.5f, -1.4f), new Vector2(-0.45f, -1.2f), 85f, 8f, 10f, PuffKind.Smoke);
        private static readonly PuffBurst DigHaze = new PuffBurst(1, new Vector2(120f, 26f), new Vector2(-0.1f, -0.4f), new Vector2(-0.05f, -0.05f), 120f, 16f, 16f, PuffKind.Smoke);

        private readonly ArtParts _parts;
        private readonly ArmMotion _motion;
        private readonly EffectsPainter _effects;
        private readonly List<Chunk> _inbound = new List<Chunk>();
        private readonly List<Vector3> _dust = new List<Vector3>();

        public ArmPainter(WorldScene scene, ArmMotion motion, EffectsPainter effects, ArtParts parts)
            : base(scene)
        {
            _parts = parts;
            _motion = motion;
            _effects = effects;
        }

        private struct Chunk
        {
            public float X;
            public float Y;
            public float Rotation;
            public float Lane;
            public Color QualityColor;
        }

        public void Clear()
        {
            _inbound.Clear();
            _motion.Clear();
        }

        public void Step(in ArmVisual arm, World world, float? stowFrames, float deltaFrames)
        {
            _motion.Step(arm, world, stowFrames);
            StepInbound(deltaFrames);
        }

        public void Draw()
        {
            _dust.Clear();
            DrawArm(_motion.Pose);
        }

        public void DrawFront()
        {
            foreach (Chunk chunk in _inbound)
            {
                DrawChunk(chunk, _parts.Art.Effects.InboundChunk, Color.white);
            }

            for (int i = 0; i < _dust.Count; i++)
            {
                Vector3 dust = _dust[i];
                _parts.Disc(dust.x, dust.y, dust.z, Palette.DigDust[i % Palette.DigDust.Length].WithAlpha(0.34f), DrawBand.Arm);
            }

            foreach (Chunk chunk in _inbound)
            {
                DrawChunk(chunk, _parts.Art.Effects.InboundMark, chunk.QualityColor);
            }
        }

        private void DrawArm(ArmPose pose)
        {
            ArmBones bones = _motion.Bones;
            Vector2 s = _motion.Shoulder;
            float c1 = Mathf.Cos(pose.ShoulderAngle);
            float s1 = Mathf.Sin(pose.ShoulderAngle);
            float c2 = Mathf.Cos(pose.ElbowAngle);
            float s2 = Mathf.Sin(pose.ElbowAngle);
            Vector2 e = new Vector2(s.x + c1 * bones.UpperArm, s.y + s1 * bones.UpperArm);
            Vector2 w = new Vector2(e.x + c2 * bones.Forearm, e.y + s2 * bones.Forearm);
            Vector2 On1(float u, float v) => new Vector2(s.x + c1 * u - s1 * v, s.y + s1 * u + c1 * v);
            Vector2 On2(float u, float v) => new Vector2(e.x + c2 * u - s2 * v, e.y + s2 * u + c2 * v);
            ArmArt art = _parts.Art.Arm;
            Part(art.Mount, s, 0f);
            Cylinder(s + bones.LiftRoot, On1(bones.LiftTip.x, bones.LiftTip.y));
            Canvas.Save();
            Canvas.Translate(e.x, e.y);
            Canvas.Rotate(pose.ElbowAngle);
            _parts.Put(art.Fore, DrawBand.Arm);
            if (pose.IsCarrying)
            {
                Part(art.Bundle, bones.Bundle, 0f);
            }

            float angle = ClawClosed + pose.Open * ClawOpenRange;
            Part(art.ClawTop, bones.ClawTop, -angle);
            Part(art.ClawBottom, bones.ClawBottom, angle);
            _parts.Put(art.Hose, DrawBand.Arm);
            Canvas.Restore();
            Part(art.Upper, s, pose.ShoulderAngle);
            Cylinder(On1(bones.BendRoot.x, bones.BendRoot.y), On2(bones.BendTip.x, bones.BendTip.y));
            Part(art.ShoulderPin, s, 0f);
            Part(art.ElbowPin, e, 0f);
            float tipX = w.x + c2 * bones.Grip;
            float tipY = w.y + s2 * bones.Grip;
            ArmEffects(pose, tipX, tipY, c2, s2);
        }

        private void ArmEffects(ArmPose pose, float tipX, float tipY, float c2, float s2)
        {
            if (pose.IsDigging)
            {
                if (Scene.IsEvery(3))
                {
                    _effects.Emit(DigSparks, new Vector2(tipX - 8f, tipY));
                }

                if (Scene.IsEvery(5))
                {
                    _effects.Emit(DigDebris, new Vector2(tipX, tipY));
                }

                if (Scene.IsEvery(22))
                {
                    _effects.AddDigRing(GashTarget.x, GashTarget.y, 10f, 0.55f);
                }
            }

            DigDust(tipX, tipY, c2, s2, pose.IsDigging);

            if (_motion.HasStartedPulling)
            {
                _effects.Emit(PulledDebris, new Vector2(GashLeft + GashOffset.x, 565f + GashOffset.y));
            }

            if (_motion.HasReleased)
            {
                float dropHeight = _motion.DropHeight;
                for (int k = 0; k < PileOffsets.Length; k++)
                {
                    float ox = PileOffsets[k][0];
                    float oy = PileOffsets[k][1];
                    _inbound.Add(new Chunk { X = tipX + c2 * ox - s2 * oy, Y = tipY + s2 * ox + c2 * oy, Rotation = pose.ElbowAngle + k * 0.7f, Lane = dropHeight + (k % 3 - 1) * 3f, QualityColor = PileQualityColors[k] });
                }
            }
        }

        private void StepInbound(float deltaFrames)
        {
            float inner = Scene.WallR + 2f;
            for (int i = _inbound.Count - 1; i >= 0; i--)
            {
                Chunk chunk = _inbound[i];
                chunk.X -= InboundPixelsPerFrame * deltaFrames;
                chunk.Y = Mathf.Lerp(chunk.Y, chunk.Lane, 1f - Mathf.Pow(0.8f, deltaFrames));
                if (chunk.X <= inner)
                {
                    _inbound.RemoveAt(i);
                    continue;
                }

                _inbound[i] = chunk;
            }
        }

        private void DigDust(float x, float y, float dx0, float dy0, bool isDigging)
        {
            float cx = x + dx0 * 8f;
            float cy = y + dy0 * 8f;
            if (!isDigging || _motion.IsDefeated)
            {
                return;
            }

            if (Scene.IsEvery(3))
            {
                _effects.Emit(DigSmoke, new Vector2(cx, cy));
            }

            if (Scene.IsEvery(4))
            {
                _effects.Emit(DigHaze, new Vector2(cx, cy + 21f));
            }

            for (int i = 0; i < 5; i++)
            {
                float p = Scene.ElapsedFrames * 0.06f + i * 2.4f;
                float r = 13f + (i % 3) * 3f;
                _dust.Add(new Vector3(cx + Mathf.Cos(p) * 10f, cy + Mathf.Sin(p * 1.2f) * 9f, r));
            }
        }

        private void DrawChunk(in Chunk chunk, ArtLayer art, Color tint)
        {
            Canvas.Save();
            Canvas.Translate(chunk.X, chunk.Y);
            Canvas.Rotate(chunk.Rotation);
            tint.a *= Mathf.Clamp01((chunk.X - Scene.WallR) / InboundFadePixels);
            _parts.Put(art, DrawBand.Arm, tint);
            Canvas.Restore();
        }

        private void Cylinder(Vector2 a, Vector2 b)
        {
            TwoBoneLimb.DrawCylinder(_parts, _parts.Art.Arm.CylinderRod, _parts.Art.Arm.CylinderTube, a, b, DrawBand.Arm);
        }

        private void Part(ArtLayer part, Vector2 p, float angle)
        {
            _parts.PutAt(part, p, angle, DrawBand.Arm);
        }
    }
}
