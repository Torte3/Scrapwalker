using UnityEngine;

namespace Scrapwalker.Game.MainView.Effects
{
    public readonly struct PuffBurst
    {
        public PuffBurst(int count, Vector2 spread, Vector2 vx, Vector2 vy, float lifeFrames, float radius, float radiusRandom, PuffKind kind)
        {
            Count = count;
            Spread = spread;
            Vx = vx;
            Vy = vy;
            LifeFrames = lifeFrames;
            Radius = radius;
            RadiusRandom = radiusRandom;
            Kind = kind;
        }

        public int Count { get; }
        public Vector2 Spread { get; }
        public Vector2 Vx { get; }
        public Vector2 Vy { get; }
        public float LifeFrames { get; }
        public float Radius { get; }
        public float RadiusRandom { get; }
        public PuffKind Kind { get; }

        public PuffBurst Spreading(Vector2 spread)
        {
            return new PuffBurst(Count, spread, Vx, Vy, LifeFrames, Radius, RadiusRandom, Kind);
        }
    }
}
