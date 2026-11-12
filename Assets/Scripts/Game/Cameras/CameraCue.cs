using UnityEngine;

namespace Scrapwalker.Game.Cameras
{
    public readonly struct CameraCue
    {
        public CameraCue(bool isMoving, float riseProgress, float baseCenterPixels, bool isLaunching, Vector2 pan, Vector2 shake)
        {
            IsMoving = isMoving;
            RiseProgress = riseProgress;
            BaseCenterPixels = baseCenterPixels;
            IsLaunching = isLaunching;
            Pan = pan;
            Shake = shake;
        }

        public bool IsMoving { get; }
        public float RiseProgress { get; }
        public float BaseCenterPixels { get; }
        public bool IsLaunching { get; }
        public Vector2 Pan { get; }
        public Vector2 Shake { get; }
    }
}
