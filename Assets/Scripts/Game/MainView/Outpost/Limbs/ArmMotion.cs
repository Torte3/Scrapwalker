using Scrapwalker.Core.Root;
using Scrapwalker.Game.Look.Art.Sets;
using Scrapwalker.Game.MainView.Compose;
using UnityEngine;
using static Scrapwalker.Game.Look.Pen;
using static Scrapwalker.Game.MainView.WorldScene;

namespace Scrapwalker.Game.MainView.Outpost.Limbs
{
    public sealed class ArmMotion
    {
        private const float WristStopDistance = 16f;
        private const float HaulFrames = 180f;
        private const float PullStart = 20f;
        private const float PushStart = 88f;
        private const float OpenStart = 100f;
        private const float OpenFrames = 10f;
        private const float Release = 102f;
        private static readonly HaulKey[] HaulPath =
        {
            new HaulKey(0f, arm => arm.DugWrist),
            new HaulKey(PullStart, arm => arm.DugWrist),
            new HaulKey(50f, arm => arm.PulledWrist),
            new HaulKey(PushStart, arm => arm.FrontOfWallWrist),
            new HaulKey(OpenStart, arm => arm.ReleaseWrist),
            new HaulKey(140f, arm => arm.ReleaseWrist),
            new HaulKey(175f, arm => arm.ReadyWrist),
        };
        private const float StowShoulderAngle = -1.35f;
        private const float StowElbowAngle = 1.25f;
        private const float StowFrames = 50f;
        private const float ShoulderOutX = 2f;
        private const float ShoulderDropY = 26f;
        private const float ReadyOutX = 82f;
        private const float ReadyY = 488f;
        private const float CloseClawFrom = 0.76f;
        private const float CloseClawSpan = 0.24f;
        private const float PulledShare = 0.73f;
        private const float FrontOfWallDx = 26f;
        private const float FrontOfWallDy = -8f;

        public const float DropGap = 34f;
        private const float DropLift = 33f;

        private readonly WorldScene _scene;
        private readonly OutpostMotion _outpostMotion;
        private ArmPose _stowFrom;
        private ArmPose _lastArm;
        private bool _hasLastArm;
        private ArmVisual _now;
        private World _world;
        private float? _lastHaulFrames;
        private float? _haulFrames;

        public ArmMotion(WorldScene scene, OutpostMotion outpostMotion, ArmArt arm)
        {
            _scene = scene;
            _outpostMotion = outpostMotion;
            Bones = new ArmBones(arm);
        }

        public ArmBones Bones { get; }

        public ArmPose Pose { get; private set; }

        public bool IsCarrying => Pose.IsCarrying;
        public bool IsDigging => Pose.IsDigging;
        public bool IsReaching => _world != null && _world.InCombat && _now.Stage.IsExtending && _now.StageProgress > 0f;
        public bool IsDefeated => _world != null && _world.IsDefeated;

        public bool HasStartedPulling => HaulCrossed(PullStart);
        public bool HasReleased => HaulCrossed(Release);

        public Vector2 Shoulder => new Vector2(_scene.WallR + ShoulderOutX, FloorY(4f) + ShoulderDropY);

        public static float DropY(int intakeRow) => FloorY(intakeRow) - DropLift;

        public float DropHeight => DropY((int)_world.Intake.Y);

        private Vector2 ReadyWrist => new Vector2(_scene.WallR + ReadyOutX, ReadyY);
        private Vector2 DugWrist => WristIn(1f);
        private Vector2 PulledWrist => WristIn(PulledShare);
        private Vector2 FrontOfWallWrist => TipPose(new Vector2(_scene.WallR + DropGap + FrontOfWallDx, DropHeight + FrontOfWallDy));
        private Vector2 ReleaseWrist => TipPose(new Vector2(_scene.WallR + DropGap, DropHeight));

        public void Clear()
        {
            _lastHaulFrames = null;
            _haulFrames = null;
        }

        public void BeginStow()
        {
            _stowFrom = _hasLastArm ? _lastArm : IK(ReadyWrist);
        }

        public void Step(in ArmVisual arm, World world, float? stowFrames)
        {
            _now = arm;
            _world = world;
            _lastHaulFrames = _haulFrames;
            _haulFrames = _now.Stage.IsHauling ? Mathf.Clamp01(_now.StageProgress) * HaulFrames : (float?)null;
            Pose = CurrentPose(stowFrames);
            if (!stowFrames.HasValue)
            {
                _lastArm = Pose;
                _hasLastArm = true;
            }
        }

        private bool HaulCrossed(float mark)
        {
            bool wasBefore = !_lastHaulFrames.HasValue || _lastHaulFrames.Value < mark;
            bool isPast = _haulFrames.HasValue && _haulFrames.Value >= mark;
            return wasBefore && isPast;
        }

        private Vector2 TipPose(Vector2 target)
        {
            Vector2 wrist = new Vector2(target.x, target.y - Bones.Grip);
            for (int i = 0; i < 20; i++)
            {
                float a2 = IK(wrist).ElbowAngle;
                wrist = new Vector2(target.x - Mathf.Cos(a2) * Bones.Grip, target.y - Mathf.Sin(a2) * Bones.Grip);
            }

            return wrist;
        }

        private Vector2 WristIn(float k)
        {
            Vector2 s = Shoulder;
            Vector2 g = GashTarget;
            float theta = Direction(s, g);
            float d = (Vector2.Distance(g, s) - WristStopDistance) * k;
            return new Vector2(s.x + Mathf.Cos(theta) * d, s.y + Mathf.Sin(theta) * d);
        }

        private ArmPose IK(Vector2 wrist)
        {
            Vector2 s = Shoulder;
            Vector2 e = TwoBoneLimb.Joint(s, wrist, Bones.UpperArm, Bones.Forearm, -1f);
            return new ArmPose { ShoulderAngle = Direction(s, e), ElbowAngle = Direction(e, wrist) };
        }

        private ArmPose CurrentPose(float? stowFrames)
        {
            if (stowFrames.HasValue)
            {
                float k = Ease(Mathf.Clamp01(stowFrames.Value / StowFrames));
                return new ArmPose { ShoulderAngle = Mathf.Lerp(_stowFrom.ShoulderAngle, StowShoulderAngle, k), ElbowAngle = Mathf.Lerp(_stowFrom.ElbowAngle, StowElbowAngle, k) };
            }

            if (_outpostMotion.IsLanding)
            {
                return new ArmPose { ShoulderAngle = StowShoulderAngle, ElbowAngle = StowElbowAngle };
            }

            if (_outpostMotion.IsUnfolding)
            {
                ArmPose ready = IK(ReadyWrist);
                float k = Ease(Mathf.Clamp01(_outpostMotion.FramesIntoUnfold / OutpostMotion.UnfoldFrames));
                return new ArmPose { ShoulderAngle = Mathf.Lerp(StowShoulderAngle, ready.ShoulderAngle, k), ElbowAngle = Mathf.Lerp(StowElbowAngle, ready.ElbowAngle, k), Open = k };
            }

            if (!_world.InCombat)
            {
                return RestPose();
            }

            return _now.Stage.PoseOf(this);
        }

        internal ArmPose RestPose()
        {
            return Reach(ReadyWrist, 1f, false);
        }

        private ArmPose Reach(Vector2 wrist, float open, bool isCarrying)
        {
            ArmPose pose = IK(wrist);
            pose.Open = open;
            pose.IsCarrying = isCarrying;
            return pose;
        }

        internal ArmPose ReachingPose()
        {
            float k = Ease(Mathf.Clamp01(_now.StageProgress));
            Vector2 pulled = PulledWrist;
            Vector2 wrist = k < 0.5f ? Vector2.Lerp(ReadyWrist, pulled, k * 2f) : Vector2.Lerp(pulled, DugWrist, k * 2f - 1f);
            return Reach(wrist, 1f - Mathf.Clamp01((k - CloseClawFrom) / CloseClawSpan), false);
        }

        internal ArmPose DiggingPose()
        {
            Vector2 wrist = DugWrist + new Vector2((_scene.Random() - 0.5f) * 4f, (_scene.Random() - 0.5f) * 3f);
            ArmPose pose = Reach(wrist, 0f, false);
            pose.IsDigging = true;
            return pose;
        }

        internal ArmPose HaulingPose()
        {
            float t = _haulFrames.Value;
            float open = t < PullStart ? 0f : 0.3f + 0.7f * Mathf.Clamp01((t - OpenStart) / OpenFrames);
            return Reach(HaulWrist(t), open, t >= PullStart && t < Release);
        }

        private Vector2 HaulWrist(float t)
        {
            for (int i = 0; i + 1 < HaulPath.Length; i++)
            {
                HaulKey from = HaulPath[i];
                HaulKey to = HaulPath[i + 1];
                if (t < to.Frame)
                {
                    return Vector2.Lerp(from.Spot(this), to.Spot(this), Ease((t - from.Frame) / (to.Frame - from.Frame)));
                }
            }

            return HaulPath[HaulPath.Length - 1].Spot(this);
        }

        private readonly struct HaulKey
        {
            public readonly float Frame;
            public readonly System.Func<ArmMotion, Vector2> Spot;

            public HaulKey(float frame, System.Func<ArmMotion, Vector2> spot)
            {
                Frame = frame;
                Spot = spot;
            }
        }
    }
}
