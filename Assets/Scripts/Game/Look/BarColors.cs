using UnityEngine;

namespace Scrapwalker.Game.Look
{
    public readonly struct BarColors
    {
        public BarColors(Color front, Color ready)
        {
            Front = front;
            Ready = ready;
        }

        public Color Front { get; }
        public Color Ready { get; }
    }
}
