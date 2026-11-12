namespace Scrapwalker.Core.Framework
{
    public sealed class ElapsedTicks
    {
        public int Ticks { get; private set; }

        public float Seconds(float tickSeconds) => Ticks * tickSeconds;

        internal void Tick()
        {
            Ticks++;
        }

        internal void Reset()
        {
            Ticks = 0;
        }
    }
}
