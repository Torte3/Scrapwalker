using Scrapwalker.Core.Framework;
using Scrapwalker.Core.Outpost.Roles;
using Scrapwalker.Core.Root;

namespace Scrapwalker.Core.Outpost.Power
{
    public sealed class EnergyStore
    {
        private readonly World _world;

        internal EnergyStore(World world)
        {
            _world = world;
        }

        public float Stored { get; internal set; }

        public float Capacity => _world.Configuration.EnergyCapacity + _world.Facilities.SumOperational(FacilityStat.EnergyCapacity);

        public float Fill => Gauge.Fill(Stored, Capacity);

        public bool CanSupply => Stored > 0f;

        internal void Tick()
        {
            Stored = Gauge.Cap(Stored, Capacity);
            Add(_world.Facilities.SumOperational(FacilityStat.SolarOutput) * _world.Configuration.TickSeconds);
        }

        internal EnergyMemento CreateMemento()
        {
            return new EnergyMemento { Stored = Stored };
        }

        internal void Restore(EnergyMemento memento)
        {
            Stored = memento.Stored;
        }

        internal void Add(float amount)
        {
            Stored = Gauge.Cap(Stored + amount, Capacity);
        }

        internal float Take(float amount)
        {
            float taken = Stored < amount ? Stored : amount;
            Stored -= taken;
            return taken;
        }
    }
}
