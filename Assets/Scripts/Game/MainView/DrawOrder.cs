using System;

namespace Scrapwalker.Game.MainView
{
    public sealed class DrawOrder
    {
        private readonly int[] _next = new int[Enum.GetValues(typeof(DrawBand)).Length];

        public void Begin()
        {
            Array.Clear(_next, 0, _next.Length);
        }

        public int Next(DrawBand band)
        {
            return _next[(int)band]++;
        }
    }
}
