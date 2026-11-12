namespace Scrapwalker.Core.Framework
{
    public sealed class SecondsCountdown
    {
        private const float EndTolerance = 0.0001f;

        public float Total { get; private set; }
        public float Elapsed { get; private set; }
        public float Progress => Total <= 0f ? 1f : System.Math.Min(1f, Elapsed / Total);
        public bool IsDone => Elapsed >= Total - EndTolerance;

        internal void Start(float seconds)
        {
            Total = seconds;
            Elapsed = 0f;
        }

        internal bool Advance(float seconds)
        {
            Elapsed += seconds;
            return IsDone;
        }
    }
}
