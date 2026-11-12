namespace Scrapwalker.Core.Framework
{
    public sealed class Countdown
    {
        public int Total { get; private set; }
        public int Remaining { get; private set; }
        public float Carry { get; private set; }

        public int Elapsed => Total - Remaining;
        public float Progress => Total > 0 ? 1f - (float)Remaining / Total : 1f;
        public bool IsDone => Remaining <= 0;

        internal void Start(int ticks)
        {
            Set(ticks, ticks, 0f);
        }

        internal void Restart()
        {
            Start(Total);
        }

        internal void Extend(int ticks)
        {
            Remaining += ticks;
            Total = Remaining;
        }

        internal void End()
        {
            Remaining = 0;
            Carry = 0f;
        }

        internal CountdownMemento CreateMemento()
        {
            return new CountdownMemento { Total = Total, Remaining = Remaining, Carry = Carry };
        }

        internal void Restore(CountdownMemento memento)
        {
            Set(memento.Total, memento.Remaining, memento.Carry);
        }

        private void Set(int total, int remaining, float carry)
        {
            Total = total;
            Remaining = remaining;
            Carry = carry;
        }

        internal bool CountDownOne()
        {
            Remaining--;
            return IsDone;
        }

        internal bool TryFillTick(float speed)
        {
            Carry += speed;
            if (Carry < 1f)
            {
                return false;
            }

            Carry -= 1f;
            return true;
        }

        internal bool Advance(float speed)
        {
            Carry += speed;
            while (Carry >= 1f && Remaining > 0)
            {
                Carry -= 1f;
                Remaining--;
            }

            return IsDone;
        }
    }
}
