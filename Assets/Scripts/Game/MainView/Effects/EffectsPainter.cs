using Scrapwalker.Game.Definitions;
using Scrapwalker.Game.Look;
using UnityEngine;

namespace Scrapwalker.Game.MainView.Effects
{
    public sealed class EffectsPainter : WorldPainter
    {
        private static readonly SparkBurst BoomEmbers = new SparkBurst(16, Vector2.zero, new Vector2(-2.5f, 2.5f), new Vector2(0f, -4f), 40f, 20f, Palette.Amber, 2f, 3f, 0.12f);
        private static readonly SparkBurst BoomSparks = BoomEmbers.Colored(Palette.Spark).Counted(8);
        private static readonly PuffBurst BoomSmoke = new PuffBurst(5, new Vector2(14f, -8f), new Vector2(-0.4f, 0.4f), new Vector2(-0.4f, -0.9f), 90f, 5f, 5f, PuffKind.Smoke);

        private readonly ParticleEffects _effects;

        public EffectsPainter(WorldScene scene, ParticleEffects effects)
            : base(scene)
        {
            _effects = effects;
        }

        public void Clear()
        {
            _effects.Clear();
        }

        public void ClearRings()
        {
            _effects.ClearRings();
        }

        private bool IsPaused => Scene.StepFrames <= 0f;

        public void Spark(float x, float y, float vx, float vy, float lifeFrames, in SparkLook look)
        {
            if (!IsPaused)
            {
                _effects.Spark(Canvas.ToCanvas(x, y), new Vector2(vx, vy), lifeFrames, look);
            }
        }

        public void Emit(in SparkBurst burst, Vector2 at)
        {
            Emit(burst, at, Vector2.zero);
        }

        public void Emit(in SparkBurst burst, Vector2 at, Vector2 push)
        {
            var look = new SparkLook(burst.Color, burst.Size, burst.Gravity);
            for (int i = 0; i < burst.Count; i++)
            {
                float x = at.x + Spread(burst.XSpread);
                float y = at.y + Spread(burst.YSpread);
                float vx = push.x + Between(burst.VxMin, burst.VxMax);
                float vy = push.y + Between(burst.VyMin, burst.VyMax);
                float life = burst.LifeFrames + Between(0f, burst.LifeRandom);
                float size = burst.Size + Between(0f, burst.SizeRandom);
                Spark(x, y, vx, vy, life, burst.SizeRandom != 0f ? new SparkLook(burst.Color, size, burst.Gravity) : look);
            }
        }

        public void Emit(in PuffBurst burst, Vector2 at)
        {
            for (int i = 0; i < burst.Count; i++)
            {
                float x = at.x + Spread(burst.Spread.x);
                float y = at.y + Spread(burst.Spread.y);
                float vx = Between(burst.Vx.x, burst.Vx.y);
                float vy = Between(burst.Vy.x, burst.Vy.y);
                Puff(x, y, vx, vy, burst.LifeFrames, burst.Radius + Between(0f, burst.RadiusRandom), burst.Kind);
            }
        }

        private float Spread(float width)
        {
            return width != 0f ? (Scene.Random() - 0.5f) * width : 0f;
        }

        private float Between(float from, float to)
        {
            return from != to ? Mathf.Lerp(from, to, Scene.Random()) : from;
        }

        public void Puff(float x, float y, float vx, float vy, float lifeFrames, float radius, PuffKind kind)
        {
            if (!IsPaused)
            {
                _effects.Puff(Canvas.ToCanvas(x, y), new Vector2(vx, vy), lifeFrames, radius, kind);
            }
        }

        public void AddDigRing(float x, float y, float radius, float alpha)
        {
            _effects.Ring(Canvas.ToCanvas(x, y), radius, alpha, 3.4f, 0.006f, Palette.Red);
        }

        public void Boom(float x, float y)
        {
            Emit(BoomEmbers, new Vector2(x, y));
            Emit(BoomSparks, new Vector2(x, y));
            _effects.Ring(Canvas.ToCanvas(x, y), 4f, 0.8f, 2f, 0.04f, Palette.Amber);
            Emit(BoomSmoke, new Vector2(x, y - 4f));

            _effects.Burst(Canvas.ToCanvas(x, y));
        }

        public void Burst(float x, float y)
        {
            _effects.Burst(Canvas.ToCanvas(x, y));
        }

        public void OrderMotes(DrawOrder order)
        {
            _effects.OrderMotes(order.Next(DrawBand.Backdrop));
        }

        public void OrderEffects(DrawOrder order)
        {
            int burst = order.Next(DrawBand.Effects);
            int puffs = order.Next(DrawBand.Effects);
            int ring = order.Next(DrawBand.Effects);
            _effects.OrderEffects(burst, puffs, ring);
        }

        public void TintMotes(Color color)
        {
            _effects.TintMotes(color);
        }

        public void Step()
        {
            _effects.Advance(Scene.StepSeconds);
        }
    }
}
