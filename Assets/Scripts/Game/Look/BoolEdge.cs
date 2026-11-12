namespace Scrapwalker.Game.Look
{
    public sealed class BoolEdge
    {
        private bool _was;

        public bool Fell { get; private set; }

        public bool Rose(bool now)
        {
            bool rose = now && !_was;
            Fell = !now && _was;
            _was = now;
            return rose;
        }
    }
}
