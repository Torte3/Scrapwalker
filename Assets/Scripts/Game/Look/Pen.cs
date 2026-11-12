using UnityEngine;

namespace Scrapwalker.Game.Look
{
    public static class Pen
    {
        public static float TwoBoneIK(float l1, float l2, float distance)
        {
            float d = Mathf.Max(Mathf.Abs(l1 - l2) + 1f, Mathf.Min(distance, l1 + l2 - 1f));
            return Mathf.Acos(Mathf.Clamp((l1 * l1 + d * d - l2 * l2) / (2f * l1 * d), -1f, 1f));
        }

        public static float Direction(Vector2 from, Vector2 to)
        {
            return Mathf.Atan2(to.y - from.y, to.x - from.x);
        }

        public static float Ease(float t)
        {
            return t < 0.5f ? 2f * t * t : 1f - Mathf.Pow(-2f * t + 2f, 2f) / 2f;
        }

        public static float Smooth(float t)
        {
            return t * t * (3f - 2f * t);
        }

        public static float Ramp(float t, float from, float to)
        {
            if (to > from)
            {
                return Mathf.Clamp01((t - from) / (to - from));
            }

            return t >= to ? 1f : 0f;
        }

        public static float SmoothRamp(float t, float from, float span)
        {
            return Smooth(Mathf.Clamp01((t - from) / span));
        }

        public static float Hash(float n)
        {
            float s = Mathf.Sin(n * 127.1f + 311.7f) * 43758.5453f;
            return s - Mathf.Floor(s);
        }
    }
}
