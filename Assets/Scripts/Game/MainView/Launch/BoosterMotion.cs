using Scrapwalker.Game.MainView.Compose;
using Scrapwalker.Game.MainView.Outpost.Limbs;
using UnityEngine;
using static Scrapwalker.Game.Look.Pen;
using static Scrapwalker.Game.MainView.WorldScene;

namespace Scrapwalker.Game.MainView.Launch
{
    public sealed class BoosterMotion
    {
        public const float Height = 200f;
        public const float OutPixels = 22f;
        public const float NozzleTip = -26f;

        private const float DoorFrames = 110f;
        private const float OutFrom = 95f;
        private const float OutFrames = 40f;
        private const float ChargeFrom = 130f;
        private const float StowOutFrames = 30f;
        private const float StowDoorFrom = 25f;
        private const float StowDoorFrames = 70f;
        private const float StowChargeFrames = 40f;
        private const float MaxBackViewPixels = 110f;

        private readonly WorldScene _scene;
        private float? _stowFrames;
        private float _stowDoor;
        private float _stowOut;
        private float _stowCharge;

        public BoosterMotion(WorldScene scene)
        {
            _scene = scene;
        }

        public float Door { get; private set; }
        public float Out { get; private set; }
        public float Charge { get; private set; }

        public float BackViewPixels => MaxBackViewPixels * Smooth(Door);

        public float StowFramesLeft => _stowFrames.HasValue ? Mathf.Max(0f, StowLength - _stowFrames.Value) : 0f;

        private float StowLength => _stowDoor > 0f ? StowDoorFrom + StowDoorFrames : Mathf.Max(StowOutFrames, StowChargeFrames);

        public float CenterY => (_scene.HullTop + HullFloorY) * 0.5f;

        public float BodyX => _scene.WallL - OutPixels * Out;

        public Vector2 Nozzle => new Vector2(BodyX + NozzleTip, CenterY);

        public void Clear()
        {
            Door = 0f;
            Out = 0f;
            Charge = 0f;
            _stowFrames = null;
        }

        public void Step(in LaunchVisual launch, float deltaFrames)
        {
            if (launch.IsLaunching)
            {
                Deploy(1f, 1f, 1f);
                return;
            }

            if (launch.IsCharging)
            {
                float f = launch.ChargeFrames - OutpostMotion.LandingFrames;
                float end = launch.ChargeEndFrames - OutpostMotion.LandingFrames;
                Deploy(Mathf.Clamp01(f / DoorFrames), SmoothRamp(f, OutFrom, OutFrames),
                    Mathf.Clamp01((f - ChargeFrom) / Mathf.Max(1f, end - ChargeFrom)));
                return;
            }

            Stow(deltaFrames);
        }

        private void Deploy(float door, float output, float charge)
        {
            Door = door;
            Out = output;
            Charge = charge;
            _stowFrames = null;
        }

        private void Stow(float deltaFrames)
        {
            if (Door <= 0f && Out <= 0f && Charge <= 0f)
            {
                _stowFrames = null;
                return;
            }

            if (!_stowFrames.HasValue)
            {
                _stowFrames = 0f;
                _stowDoor = Door;
                _stowOut = Out;
                _stowCharge = Charge;
            }

            float stowFrames = _stowFrames.Value + deltaFrames;
            _stowFrames = stowFrames;
            Out = _stowOut * (1f - SmoothRamp(stowFrames, 0f, StowOutFrames));
            Door = _stowDoor * (1f - Mathf.Clamp01((stowFrames - StowDoorFrom) / StowDoorFrames));
            Charge = _stowCharge * (1f - Mathf.Clamp01(stowFrames / StowChargeFrames));
            if (StowFramesLeft <= 0f)
            {
                Clear();
            }
        }
    }
}
