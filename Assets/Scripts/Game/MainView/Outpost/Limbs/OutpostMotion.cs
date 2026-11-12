using Scrapwalker.Game.MainView.Compose;
using Scrapwalker.Game.MainView.Effects;
using UnityEngine;
using static Scrapwalker.Game.Look.Pen;
using static Scrapwalker.Game.MainView.WorldScene;

namespace Scrapwalker.Game.MainView.Outpost.Limbs
{
    public sealed class OutpostMotion
    {
        public const float RiseHeight = 130f;
        public const float Stride = 180f;
        public const float SwingShare = 0.25f;
        public const float UnfoldFrames = 40f;
        private const float RiseDelayFrames = 30f;
        private const float RiseFrames = 70f;

        private const float StandUpFrames = RiseDelayFrames + RiseFrames;

        internal const float CenterShift = 315f;
        private const float CameraEaseSpan = 200f;
        private const float CameraEaseFrom = CenterShift - CameraEaseSpan * 0.5f;
        internal const float CruisePixelsPerFrame = 3.2f;
        private const float DecelerationPixels = 190f;
        private const float MinApproachSpeed = 0.9f;
        private const float AccelerationFrames = 60f;
        private const float CameraBackPixels = 520f;
        private const float LandFrames = 70f;
        private const float LandHold = 24f;
        public const float LandingFrames = LandFrames + LandHold;
        private static readonly float DecelerationExponentialFrames = DecelerationPixels / CruisePixelsPerFrame * Mathf.Log(CruisePixelsPerFrame / MinApproachSpeed);
        private static readonly float DecelerationFrames = DecelerationExponentialFrames + DecelerationPixels / CruisePixelsPerFrame;

        private static readonly PuffBurst LandingSteam = new PuffBurst(2, Vector2.zero, new Vector2(-1.1f, 1.1f), new Vector2(0f, -0.8f), 34f, 3f, 3f, PuffKind.Steam);

        private readonly WorldScene _scene;
        private readonly EffectsPainter _effects;
        private float _travelFrom;
        private float _walkFrom;
        private float _travelDistance;
        private float _approachV0;
        private float _routeProgressFrom;
        private float _destinationWorld;
        private float _landFrom;
        private float _paceV;
        private float _stopAheadPixels = CameraBackPixels;

        public OutpostMotion(WorldScene scene, EffectsPainter effects)
        {
            _scene = scene;
            _effects = effects;
        }

        public OutpostPhase Phase { get; private set; }

        public bool IsMoving => Phase == OutpostPhase.Holding || Phase == OutpostPhase.Walking || Phase == OutpostPhase.Approaching;

        public bool IsHolding => Phase == OutpostPhase.Holding;

        public float FramesSinceStart { get; private set; }
        public float FramesIntoMove { get; private set; }
        public float WalkV { get; private set; }
        public float GaitPhase { get; private set; }

        public bool IsLanding => Phase == OutpostPhase.Landing;
        public float FramesIntoLanding { get; private set; }
        public bool IsUnfolding => Phase == OutpostPhase.Unfolding;
        public float FramesIntoUnfold { get; private set; }

        public bool HasTouchedDown => IsLanding && FramesIntoLanding >= LandFrames;

        public bool HasStoodUp => IsMoving && FramesIntoMove >= StandUpFrames;

        public float LeftX { get; private set; }

        public void KeepRuinBehind()
        {
            LeftX = _scene.RuinOff;
        }

        public float AheadX => Phase == OutpostPhase.Approaching || IsLanding ? _destinationWorld : _scene.RuinOff;

        public void Reset()
        {
            GaitPhase = 0f;
            FramesIntoMove = 0f;
            WalkV = 0f;
            if (Phase == OutpostPhase.Approaching)
            {
                Phase = OutpostPhase.Walking;
            }
            else if (IsLanding)
            {
                Phase = OutpostPhase.Resting;
            }

            _scene.Rise = 0f;
            _scene.BaseX = 0f;
            _scene.BaseY = 0f;
            _scene.WorldX = -_scene.WalkDistance;
            _scene.RuinOff = _scene.WalkDistance;
            _travelFrom = 0f;
            _travelDistance = 0f;
            _destinationWorld = 0f;
            _paceV = 0f;
            _stopAheadPixels = CameraBackPixels;
        }

        public void Start()
        {
            Phase = OutpostPhase.Walking;
            GaitPhase = 0f;
            _walkFrom = _scene.WalkDistance;
            FramesSinceStart = 0f;
            FramesIntoMove = 0f;
        }

        public void StartCruising()
        {
            Start();
            FramesIntoMove = StandUpFrames + AccelerationFrames;
            FramesSinceStart = FramesIntoMove;
            _paceV = CruisePixelsPerFrame;
            WalkV = _paceV;
            _stopAheadPixels = CameraBackPixels;
        }

        public void Hold()
        {
            Reset();
            Start();
            Phase = OutpostPhase.Holding;
        }

        public void Release()
        {
            if (IsHolding)
            {
                Phase = OutpostPhase.Walking;
            }
        }

        public void Stop()
        {
            if (IsMoving)
            {
                Phase = OutpostPhase.Resting;
            }
        }

        public bool BeginLanding()
        {
            if (Phase != OutpostPhase.Approaching)
            {
                if (IsUnfolding)
                {
                    Phase = OutpostPhase.Resting;
                }

                return false;
            }

            _landFrom = _scene.Rise;
            Phase = OutpostPhase.Landing;
            FramesIntoLanding = 0f;
            return true;
        }

        public bool StepLanding(float deltaFrames)
        {
            FramesIntoLanding += deltaFrames;
            float k = Mathf.Clamp01(FramesIntoLanding / LandFrames);
            _scene.Rise = _landFrom * (1f - Ease(k));
            _scene.BaseY = -_scene.Rise;
            if (k < 1f && _scene.IsEvery(3))
            {
                _effects.Emit(LandingSteam.Spreading(new Vector2(_scene.HullWidth - 80f, 0f)), new Vector2(HullLeft + _scene.HullWidth * 0.5f + _scene.BaseX, Ground - 2f));
            }

            if (FramesIntoLanding < LandingFrames)
            {
                return false;
            }

            float destination = _destinationWorld;
            Reset();
            _scene.RuinOff = destination;
            Phase = OutpostPhase.Unfolding;
            FramesIntoUnfold = 0f;
            return true;
        }

        public void StepUnfold(float deltaFrames)
        {
            FramesIntoUnfold += deltaFrames;
            if (FramesIntoUnfold >= UnfoldFrames)
            {
                Phase = OutpostPhase.Resting;
            }
        }

        public void StepMove(float deltaFrames, in TravelVisual travel, float holdFrames)
        {
            FramesSinceStart += deltaFrames;
            if (holdFrames > 0f && FramesIntoMove <= 0f)
            {
                return;
            }

            FramesIntoMove += deltaFrames;
            _scene.Rise = RiseHeight * Ease(Mathf.Clamp01((FramesIntoMove - RiseDelayFrames) / RiseFrames));
            if (travel.HasRoute)
            {
                StepApproach(deltaFrames, travel);
            }
            else
            {
                StepCruise(deltaFrames);
            }

            float ramp = CameraFollow(_scene.WalkDistance - _walkFrom);
            float back = CenterShift * Ease(Mathf.Clamp01(_stopAheadPixels / CameraBackPixels));
            _scene.BaseX = Mathf.Min(ramp, back);
            _scene.WorldX = _scene.BaseX - _scene.WalkDistance;
            _scene.BaseY = -_scene.Rise;
            GaitPhase += WalkV * deltaFrames * (1f - SwingShare) / Stride;
        }

        private void StepApproach(float deltaFrames, in TravelVisual travel)
        {
            float frames = travel.RouteFrames;
            if (Phase != OutpostPhase.Approaching)
            {
                Phase = OutpostPhase.Approaching;
                _routeProgressFrom = Mathf.Min(travel.RouteProgress, 0.99f);
                _travelFrom = _scene.WalkDistance;
                _approachV0 = WalkV;
                _travelDistance = TravelSpan(frames * (1f - _routeProgressFrom), _approachV0);
                _destinationWorld = _travelFrom + _travelDistance;
            }

            float left = frames * (1f - _routeProgressFrom);
            float u = (travel.RouteProgress - _routeProgressFrom) / (1f - _routeProgressFrom);
            float f = u * left;
            float target = _travelFrom + _travelDistance * Covered(f, left, _approachV0);
            WalkV = deltaFrames > 0.0001f ? (target - _scene.WalkDistance) / deltaFrames : 0f;
            _paceV = _travelDistance * (Covered(f + 1f, left, _approachV0) - Covered(f, left, _approachV0));
            _scene.WalkDistance = target;
            _stopAheadPixels = _destinationWorld - _scene.WalkDistance;
        }

        private void StepCruise(float deltaFrames)
        {
            Phase = OutpostPhase.Walking;
            float standing = CruisePixelsPerFrame * Mathf.Clamp01((FramesIntoMove - StandUpFrames) / AccelerationFrames);
            _paceV = Mathf.Min(standing, Mathf.MoveTowards(_paceV, CruisePixelsPerFrame, CruisePixelsPerFrame / AccelerationFrames * deltaFrames));
            WalkV = _paceV;
            _scene.WalkDistance += WalkV * deltaFrames;
            _stopAheadPixels = Mathf.Min(CameraBackPixels, _stopAheadPixels + WalkV * deltaFrames);
        }

        private static float CameraFollow(float d)
        {
            if (d < CameraEaseFrom)
            {
                return d;
            }

            if (d > CameraEaseFrom + CameraEaseSpan)
            {
                return CenterShift;
            }

            float x = d - CameraEaseFrom;
            return CameraEaseFrom + x - x * x / (2f * CameraEaseSpan);
        }

        private static float DecelerationCovered(float g)
        {
            if (g <= DecelerationExponentialFrames)
            {
                return DecelerationPixels * (1f - Mathf.Exp(-g * CruisePixelsPerFrame / DecelerationPixels));
            }

            return Mathf.Min(DecelerationPixels, DecelerationPixels - MinApproachSpeed * DecelerationPixels / CruisePixelsPerFrame + MinApproachSpeed * (g - DecelerationExponentialFrames));
        }

        private readonly struct TravelSegments
        {
            public TravelSegments(float frames, float v0)
            {
                float acceleration = OutpostMotion.AccelerationFrames * Mathf.Clamp01(1f - v0 / CruisePixelsPerFrame);
                float deceleration = OutpostMotion.DecelerationFrames;
                float sum = acceleration + deceleration;
                if (sum > frames && sum > 0f)
                {
                    float scale = frames / sum;
                    acceleration *= scale;
                    deceleration *= scale;
                }

                AccelerationFrames = acceleration;
                DecelerationFrames = deceleration;
                CruiseFrames = Mathf.Max(0f, frames - acceleration - deceleration);
                AccelerationPixels = v0 * acceleration + (CruisePixelsPerFrame - v0) * acceleration * 0.5f;
                CruisePixels = CruisePixelsPerFrame * CruiseFrames;
                DecelerationPixels = OutpostMotion.DecelerationPixels * deceleration / OutpostMotion.DecelerationFrames;
            }

            public float AccelerationFrames { get; }
            public float CruiseFrames { get; }
            public float DecelerationFrames { get; }
            public float AccelerationPixels { get; }
            public float CruisePixels { get; }
            public float DecelerationPixels { get; }
            public float TotalPixels => AccelerationPixels + CruisePixels + DecelerationPixels;
        }

        private static float TravelSpan(float frames, float v0)
        {
            return new TravelSegments(frames, v0).TotalPixels;
        }

        internal static float Covered(float f, float frames, float v0)
        {
            var segments = new TravelSegments(frames, v0);
            float total = segments.TotalPixels;
            if (total <= 0f)
            {
                return 1f;
            }

            float acceleration = segments.AccelerationFrames;
            if (f <= acceleration)
            {
                float k = acceleration > 0f ? f / acceleration : 1f;
                return (v0 * f + (CruisePixelsPerFrame - v0) * f * k * 0.5f) / total;
            }

            float cruise = segments.CruiseFrames;
            if (f <= acceleration + cruise)
            {
                return (segments.AccelerationPixels + CruisePixelsPerFrame * (f - acceleration)) / total;
            }

            float deceleration = segments.DecelerationFrames;
            float decelerationProgress = deceleration > 0f ? Mathf.Clamp01((f - acceleration - cruise) / deceleration) : 1f;
            return (segments.AccelerationPixels + segments.CruisePixels + segments.DecelerationPixels * DecelerationCovered(decelerationProgress * DecelerationFrames) / DecelerationPixels) / total;
        }
    }
}
