using System;

namespace Scrapwalker.Core.Sessions
{
    public sealed class GameTime
    {
        private const int MaxCatchUpTicks = 6;
        private const float ScaleChangePerSecond = 5f;

        private float _accumulator;

        public int Speed { get; internal set; } = 1;

        public float Scale { get; private set; } = 1f;

        public float TickShare { get; private set; }

        internal void Advance(Simulation simulation, float deltaTime, TimeFlow flow, Action afterTick)
        {
            float target = flow == TimeFlow.Stopped ? 0f : Speed;
            float fastest = Math.Max(1f, Math.Max(Speed, Scale));
            float step = deltaTime * ScaleChangePerSecond * fastest * fastest;
            Scale = Scale < target ? Math.Min(target, Scale + step) : Math.Max(target, Scale - step);
            if (Scale <= 0f)
            {
                return;
            }

            float tick = simulation.World.Configuration.TickSeconds;
            _accumulator += deltaTime * Scale;
            int ticks = 0;
            while (_accumulator >= tick && ticks < MaxCatchUpTicks)
            {
                simulation.Tick();
                afterTick?.Invoke();
                _accumulator -= tick;
                ticks++;
            }

            if (_accumulator > tick * MaxCatchUpTicks)
            {
                _accumulator = 0f;
            }

            TickShare = Math.Min(1f, _accumulator / tick);
        }
    }
}
