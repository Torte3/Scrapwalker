using System.Collections.Generic;
using Scrapwalker.Core.Root;

namespace Scrapwalker.Core.Outpost.Bots.Actions
{
    public sealed class ChargeAction : BotAction
    {
        private const float FullTolerance = 0.001f;

        public ChargeAction(Facility charger)
            : base(Place.At(charger), charger.Door)
        {
            Charger = charger;
        }

        internal ChargeAction(ChargeMemento memento, Facility charger)
            : base(memento)
        {
            Charger = charger;
        }

        public Facility Charger { get; }

        public override IEnumerable<PlaceClaim> PlaceClaims
        {
            get
            {
                yield return new PlaceClaim(Target, PlaceUse.Charge);
            }
        }

        internal override bool GivesWayToCharging => false;

        internal override BotActionMemento CreateMemento()
        {
            return Written(new ChargeMemento());
        }

        public override T Show<T>(IBotActionView<T> view)
        {
            return view.Charge(this);
        }

        internal override bool CanContinue(World world)
        {
            return !Charger.IsRemoved && Charger.IsOperational;
        }

        internal override bool UsesBattery(World world, Bot bot)
        {
            return bot.IsMoving;
        }

        internal override void Tick(World world, Bot bot)
        {
            if (!CanContinue(world))
            {
                bot.GoIdle();
                return;
            }

            if (!BotMotion.Step(world, bot))
            {
                return;
            }

            float full = world.Configuration.BatterySeconds;
            float perTick = full / world.Configuration.ChargeSeconds * world.Configuration.TickSeconds;
            float want = System.Math.Min(full - bot.BatterySeconds, perTick);
            float got = world.Energy.Take(want);
            bot.BatterySeconds += got;
            if (bot.BatterySeconds >= full - FullTolerance)
            {
                bot.BatterySeconds = full;
                bot.GoIdle();
            }
            else if (got <= 0f)
            {
                bot.GoIdle();
            }
        }
    }
}
