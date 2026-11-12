using Scrapwalker.Core.Framework;
using Scrapwalker.Core.Root;

namespace Scrapwalker.Core.Travel
{
    public sealed class Takeoff
    {
        private readonly World _world;
        private readonly Countdown _time = new Countdown();

        internal Takeoff(World world)
        {
            _world = world;
            world.Events.On<TookOff>(Begin);
        }

        public bool IsActive { get; private set; }
        public bool IsOver => IsActive && _time.IsDone;
        public int ElapsedTicks => _time.Total - _time.Remaining;

        private void Begin(TookOff tookOff)
        {
            _world.Log.Write(() => "離陸: " + tookOff.From.Key);
            IsActive = true;
            _time.Start(_world.Configuration.TicksFor(_world.Configuration.TakeoffSeconds));
        }

        internal void Tick()
        {
            if (IsActive && !IsOver && _time.CountDownOne())
            {
                _world.Emit(new TakeoffFinished());
            }
        }
    }
}
