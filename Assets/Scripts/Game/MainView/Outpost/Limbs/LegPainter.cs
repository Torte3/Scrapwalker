using Scrapwalker.Game.Definitions;
using Scrapwalker.Game.Look;
using Scrapwalker.Game.Look.Art;
using Scrapwalker.Game.Look.Art.Parts;
using Scrapwalker.Game.Look.Art.Sets;
using Scrapwalker.Game.MainView.Effects;
using UnityEngine;
using static Scrapwalker.Game.Look.Pen;
using static Scrapwalker.Game.MainView.WorldScene;

namespace Scrapwalker.Game.MainView.Outpost.Limbs
{
    public sealed class LegPainter : WorldPainter
    {
        private const float Lift = 34f;
        private const float LiftShare = 0.22f;
        private static readonly PuffBurst CylinderSteam = new PuffBurst(4, Vector2.zero, new Vector2(-0.4f, -1f), new Vector2(-0.3f, -0.7f), 40f, 3f, 2f, PuffKind.Steam);
        private static readonly SparkBurst FootDust = new SparkBurst(9, new Vector2(30f, 0f), new Vector2(-1.2f, 1.2f), new Vector2(-0.3f, -1.4f), 34f, 0f, Color.white, 2f, 3f, 0.03f);

        private readonly OutpostMotion _motion;
        private readonly EffectsPainter _effects;
        private readonly ArtParts _parts;
        private readonly LegRow _far;
        private readonly LegRow _near;

        public LegPainter(WorldScene scene, OutpostMotion motion, EffectsPainter effects, ArtParts parts)
            : base(scene)
        {
            _motion = motion;
            _effects = effects;
            _parts = parts;
            _far = new LegRow(parts.Art.FarLeg, 0.5f, FootDust.Colored(Palette.FarFootDust));
            _near = new LegRow(parts.Art.NearLeg, 0f, FootDust.Colored(Palette.NearFootDust));
        }

        private sealed class LegRow
        {
            public LegRow(LegArt art, float phaseOffset, SparkBurst dust)
            {
                Art = art;
                PhaseOffset = phaseOffset;
                Dust = dust;
                ArtRig tube = art.CylinderTube.Find<ArtRig>();
                ArtRig rod = art.CylinderRod.Find<ArtRig>();
                Thigh = art.Thigh.Find<ArtRig>().Length;
                Shin = art.Shin.Find<ArtRig>().Length;
                TubeAlong = tube.Along;
                TubeAt = tube.At;
                TubeLength = tube.Length;
                RodAlong = rod.Along;
                RodAt = rod.At;
                FootToAnkle = art.Foot.Find<ArtRig>().At;
                Hips = art.Hip.Find<ArtRig>().Points;

                WasSwinging = new bool[Hips.Length];
                Poses = new LegPose[Hips.Length];
            }

            public LegArt Art { get; }
            public float PhaseOffset { get; }
            public SparkBurst Dust { get; }
            public float Thigh { get; }
            public float Shin { get; }
            public float TubeAlong { get; }
            public Vector2 TubeAt { get; }
            public float TubeLength { get; }
            public float RodAlong { get; }
            public Vector2 RodAt { get; }
            public Vector2 FootToAnkle { get; }
            public Vector2[] Hips { get; }
            public bool[] WasSwinging { get; }
            public LegPose[] Poses { get; }
        }

        public void Reset()
        {
            System.Array.Clear(_far.WasSwinging, 0, _far.WasSwinging.Length);
            System.Array.Clear(_near.WasSwinging, 0, _near.WasSwinging.Length);
        }

        public void Draw(bool isMoving)
        {
            if (Scene.Rise > 1f)
            {
                float rx = Scene.HullWidth * 0.5f + 40f;
                float alpha = 0.28f / 0.3f * Mathf.Min(1f, Scene.Rise / OutpostMotion.RiseHeight);
                _parts.Shadow(HullLeft + Scene.HullWidth * 0.5f + Scene.BaseX, Ground + 3f, rx, 9f, new Color(1f, 1f, 1f, alpha), DrawBand.Legs);
            }

            DrawRow(_far, isMoving);
            DrawRow(_near, isMoving);
        }

        private static Vector2 FootOffset(float phase, out bool isSwinging)
        {
            float stride = OutpostMotion.Stride;
            if (phase >= OutpostMotion.SwingShare)
            {
                isSwinging = false;
                float back = (phase - OutpostMotion.SwingShare) / (1f - OutpostMotion.SwingShare);
                return new Vector2(stride * 0.5f - stride * back, 0f);
            }

            isSwinging = true;
            float k = phase / OutpostMotion.SwingShare;
            return new Vector2(-stride * 0.5f + stride * SwingForward(k), -Lift * SwingLift(k));
        }

        private static float SwingLift(float k)
        {
            if (k < LiftShare)
            {
                return 1f - Mathf.Pow(1f - k / LiftShare, 2f);
            }

            if (k > 1f - LiftShare)
            {
                return 1f - Mathf.Pow((k - (1f - LiftShare)) / LiftShare, 2f);
            }

            return 1f;
        }

        private static float SwingForward(float k)
        {
            if (k < LiftShare)
            {
                return 0f;
            }

            if (k > 1f - LiftShare)
            {
                return 1f;
            }

            return Ease((k - LiftShare) / (1f - LiftShare * 2f));
        }

        private struct LegPose
        {
            public Vector2 Hip;
            public Vector2 Knee;
            public Vector2 Ankle;
            public Vector2 Foot;
            public Vector2 CylinderRoot;
            public Vector2 CylinderTip;
        }

        private void DrawRow(LegRow row, bool isMoving)
        {
            for (int i = 0; i < row.Poses.Length; i++)
            {
                row.Poses[i] = PoseOf(i, row, isMoving);
            }

            LegArt art = row.Art;
            foreach (LegPose leg in row.Poses)
            {
                Part(art.Thigh, leg.Hip, Direction(leg.Hip, leg.Knee), 1f);
            }

            foreach (LegPose leg in row.Poses)
            {
                Part(art.Shin, leg.Knee, Direction(leg.Knee, leg.Ankle), (leg.Ankle - leg.Knee).magnitude / row.Shin);
            }

            foreach (LegPose leg in row.Poses)
            {
                _parts.PutAt(art.CylinderRod, leg.CylinderTip, Direction(leg.CylinderRoot, leg.CylinderTip), DrawBand.Legs);
            }

            foreach (LegPose leg in row.Poses)
            {
                _parts.PutAt(art.CylinderTube, leg.CylinderRoot, Direction(leg.CylinderRoot, leg.CylinderTip), DrawBand.Legs);
            }

            foreach (LegPose leg in row.Poses)
            {
                Part(art.Knee, leg.Knee, 0f, 1f);
            }

            foreach (LegPose leg in row.Poses)
            {
                Part(art.Ankle, leg.Ankle, 0f, 1f);
            }

            foreach (LegPose leg in row.Poses)
            {
                Part(art.Foot, leg.Foot, 0f, 1f);
            }

            foreach (LegPose leg in row.Poses)
            {
                Part(art.Hip, leg.Hip, 0f, 1f);
            }
        }

        private LegPose PoseOf(int i, LegRow row, bool isMoving)
        {
            float keel = Ground + Scene.BaseY;
            float phase = Mathf.Repeat(_motion.GaitPhase + i * 0.25f + row.PhaseOffset, 1f);
            Vector2 foot = FootOffset(phase, out bool isSwinging);
            float amplitude = Mathf.Min(1f, Scene.Rise / OutpostMotion.RiseHeight);
            Vector2 hip = new Vector2(HullLeft + Scene.BaseX, keel) + row.Hips[i];
            float sole = Ground + Scene.BaseY + Scene.Rise;
            var footAt = new Vector2(hip.x + foot.x * amplitude, sole + foot.y * amplitude);
            Vector2 ankle = footAt - row.FootToAnkle;
            Vector2 knee = TwoBoneLimb.Joint(hip, ankle, row.Thigh, row.Shin, 1f);
            Vector2 cylinderA = hip + (knee - hip) * row.TubeAlong + row.TubeAt;
            Vector2 cylinderB = knee + (ankle - knee) * row.RodAlong + row.RodAt;
            Vector2 steamAt = cylinderA + (cylinderB - cylinderA).normalized * row.TubeLength;
            StepEffects(i, row, isMoving, isSwinging, steamAt, footAt.x);
            return new LegPose { Hip = hip, Knee = knee, Ankle = ankle, Foot = footAt, CylinderRoot = cylinderA, CylinderTip = cylinderB };
        }

        private void StepEffects(int i, LegRow row, bool isMoving, bool isSwinging, Vector2 steamAt, float footX)
        {
            bool wasSwinging = row.WasSwinging[i];
            if (isMoving && _motion.WalkV > 0.1f)
            {
                if (isSwinging && !wasSwinging)
                {
                    _effects.Emit(CylinderSteam, steamAt);
                }

                if (!isSwinging && wasSwinging)
                {
                    _effects.Emit(row.Dust, new Vector2(footX, Ground - 2f), new Vector2(Scene.ScrollPixels, 0f));
                }
            }

            row.WasSwinging[i] = isSwinging;
        }

        private void Part(ArtLayer part, Vector2 p, float angle, float stretch)
        {
            Canvas.Save();
            Canvas.Translate(p.x, p.y);
            Canvas.Rotate(angle);
            Canvas.Scale(stretch, 1f);
            _parts.Put(part, DrawBand.Legs);
            Canvas.Restore();
        }
    }
}
