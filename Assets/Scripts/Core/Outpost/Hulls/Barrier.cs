using Scrapwalker.Core.Framework;
using Scrapwalker.Core.Outpost.Roles;
using Scrapwalker.Core.Root;

namespace Scrapwalker.Core.Outpost.Hulls
{
    public sealed class Barrier
    {
        private const float RoundingTolerance = 0.0001f;
        private const float Full = 0.999f;

        private readonly World _world;

        internal Barrier(World world)
        {
            _world = world;
        }

        public float HP { get; internal set; }

        public float Capacity => _world.Facilities.SumOperational(FacilityStat.BarrierCapacity);

        public float Fill => Gauge.Fill(HP, Capacity);
        public bool IsRecovering => Fill < Full;

        internal void Recharge()
        {
            float cap = Capacity;
            HP = Gauge.Cap(HP, cap);
            float facility = cap - HP;
            if (facility <= 0f)
            {
                return;
            }

            float want = System.Math.Min(facility, _world.Facilities.SumOperational(FacilityStat.BarrierRegeneration) * _world.Configuration.TickSeconds);
            float perPoint = _world.Configuration.BarrierEnergyPerPoint;
            float energy = _world.Energy.Take(want * perPoint);
            HP += perPoint > 0f ? energy / perPoint : want;
        }

        internal BarrierMemento CreateMemento()
        {
            return new BarrierMemento { HP = HP };
        }

        internal void Restore(BarrierMemento memento)
        {
            HP = memento.HP;
        }

        internal int Absorb(int damage)
        {
            float taken = System.Math.Min(HP, damage);
            HP -= taken;
            return damage - (int)System.Math.Ceiling(taken - RoundingTolerance);
        }
    }
}
