using UnityEngine;

namespace Scrapwalker.Game.Cameras
{
    public sealed class TitlePan
    {
        private const float SlideSeconds = 1.6f;

        private float _elapsed;

        public float Aside { get; private set; }
        public bool IsSliding { get; private set; }

        public void Hold()
        {
            Aside = 1f;
            IsSliding = false;
        }

        public void Clear()
        {
            Aside = 0f;
            IsSliding = false;
        }

        public void SlideIn()
        {
            Aside = 1f;
            _elapsed = 0f;
            IsSliding = true;
        }

        public void Tick(float deltaTime)
        {
            if (!IsSliding)
            {
                return;
            }

            _elapsed += deltaTime;
            float u = Mathf.Clamp01(_elapsed / SlideSeconds);
            Aside = 1f - Mathf.SmoothStep(0f, 1f, u);
            IsSliding = u < 1f;
        }
    }
}
