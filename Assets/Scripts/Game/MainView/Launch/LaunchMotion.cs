using UnityEngine;
using static Scrapwalker.Game.Look.Pen;

namespace Scrapwalker.Game.MainView.Launch
{
    public sealed class LaunchMotion
    {
        public const float SettleFrames = 120f;
        public const float WheelFrom = 20f;
        public const float WheelFrames = 30f;
        public const float BuildFrom = 50f;
        public const float FireAt = 170f;
        private const float PitchFrom = 275f;
        private const float PitchFrames = 160f;
        private const float MaxPitch = 0.38f;
        private const float Acceleration = 1.4f;
        private const float MaxSpeed = 140f;
        private const float LiftSpeed = 90f;
        private const float LiftPerSpeed = 0.04f;
        private const float MaxShift = 240f;
        private const float MaxCameraLag = 120f;
        private const float CameraRise = 0.3f;
        private const float BackViewReleaseFrames = 100f;
        private const float AirborneY = -4f;
        private const float WheelStowFrames = 30f;

        private readonly WorldScene _scene;
        private int _frames;
        private float _walkFrom;
        private float _wheelUp;
        private Flight _flight;
        private Flight _previous;
        private float _coastDistance;
        private float _coastHeight;

        public LaunchMotion(WorldScene scene)
        {
            _scene = scene;
        }

        public bool IsActive { get; private set; }

        public float FramesSinceLaunch { get; private set; } = -SettleFrames;
        public float Distance { get; private set; }
        public float Speed => _flight.Speed;
        public float Height { get; private set; }
        public float Pitch { get; private set; }

        public float WheelOut => SmoothRamp(FramesSinceLaunch, WheelFrom, WheelFrames) * (1f - SmoothRamp(_wheelUp, 0f, WheelStowFrames));

        public float Build => IsFired ? 0f : Mathf.Clamp01((FramesSinceLaunch - BuildFrom) / (FireAt - BuildFrom));
        public bool IsFired => FramesSinceLaunch >= FireAt;
        public float Ignition => IsFired ? Mathf.Clamp01((FramesSinceLaunch - FireAt) / 6f) : 0f;

        public float BackViewWeight => IsActive ? 1f - SmoothRamp(FramesSinceLaunch, FireAt, BackViewReleaseFrames) : 1f;

        public float StarX => _scene.WorldX - _coastDistance;
        public float StarLift => -(Height + _coastHeight) * CameraRise;

        public Vector2 Pan => IsActive ? new Vector2(Mathf.Max(0f, _scene.BaseX - MaxCameraLag), -Height * CameraRise) : Vector2.zero;

        public void Begin()
        {
            IsActive = true;
            _frames = 0;
            _walkFrom = _scene.WalkDistance;
            _wheelUp = 0f;
            FramesSinceLaunch = -SettleFrames;
            _flight = default;
            _previous = default;
            Distance = 0f;
            Height = 0f;
            Pitch = 0f;
            _coastDistance = 0f;
            _coastHeight = 0f;
        }

        public void End()
        {
            IsActive = false;
            _scene.Tilt = 0f;
            _scene.CameraLag = 0f;
        }

        public void Step(float launchFrames)
        {
            while (_frames + 1 <= launchFrames)
            {
                _previous = _flight;
                Integrate(_frames - SettleFrames);
                _frames++;
            }

            FramesSinceLaunch = launchFrames - SettleFrames;
            float share = Mathf.Clamp01(launchFrames - _frames);
            Distance = Mathf.Lerp(_previous.Distance, _flight.Distance, share);
            Height = Mathf.Lerp(_previous.Height, _flight.Height, share);
            Pitch = Mathf.Lerp(_previous.Pitch, _flight.Pitch, share);
            float baseX = Mathf.Min(Distance, MaxShift);
            _scene.BaseX = baseX;
            _scene.CameraLag = Mathf.Min(baseX, MaxCameraLag);
            _scene.WalkDistance = _walkFrom + Distance;
            _scene.WorldX = baseX - _scene.WalkDistance;
            _scene.Rise = 0f;
            _scene.BaseY = Height;
            float wheelFull = SmoothRamp(FramesSinceLaunch, WheelFrom, WheelFrames);
            _scene.Tilt = -Mathf.Atan2(18f * wheelFull, _scene.HullWidth) - Pitch;
        }

        public void Coast(float frames)
        {
            _coastDistance += (_flight.Distance - _previous.Distance) * frames;
            _coastHeight += (_flight.Height - _previous.Height) * frames;
        }

        private void Integrate(float u)
        {
            if (u < 0f)
            {
                return;
            }

            if (_flight.Height < AirborneY)
            {
                _wheelUp++;
            }

            if (u >= FireAt)
            {
                _flight.Speed = Mathf.Min(MaxSpeed, _flight.Speed + Acceleration);
            }

            if (_flight.Speed > LiftSpeed)
            {
                _flight.Height -= (_flight.Speed - LiftSpeed) * LiftPerSpeed;
            }

            _flight.Pitch = MaxPitch * SmoothRamp(u, PitchFrom, PitchFrames);
            _flight.Distance += _flight.Speed * Mathf.Cos(_flight.Pitch);
            _flight.Height -= _flight.Speed * Mathf.Sin(_flight.Pitch);
        }

        private struct Flight
        {
            public float Distance;
            public float Speed;
            public float Height;
            public float Pitch;
        }
    }
}
