using UnityEngine;

namespace Scrapwalker.Game.MainView.Effects
{
    public readonly struct SparkLook
    {
        public SparkLook(Color color, float size, float gravity)
        {
            Color = color;
            Size = size;
            Gravity = gravity;
        }

        public Color Color { get; }
        public float Size { get; }
        public float Gravity { get; }
    }
}
