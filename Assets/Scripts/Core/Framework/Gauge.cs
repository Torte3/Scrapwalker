namespace Scrapwalker.Core.Framework
{
    public static class Gauge
    {
        public static float Fill(float value, float capacity)
        {
            return capacity > 0f ? System.Math.Min(1f, value / capacity) : 0f;
        }

        public static float Cap(float value, float capacity)
        {
            return System.Math.Min(value, capacity);
        }
    }
}
