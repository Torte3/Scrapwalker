using Scrapwalker.Core.Framework;
using Scrapwalker.Core.Outpost.Bots.Haul;
using Scrapwalker.Core.Root;

namespace Scrapwalker.Core.Outpost.Bots.Actions
{
    public sealed class DropoffAction : HaulAction
    {
        private readonly Countdown _handOver = new Countdown();

        public DropoffAction(PickupAction picked)
            : base(picked.Priority, picked.Target, picked.Destination, picked.Type, picked.Spot, picked.Task, picked.Count)
        {
        }

        public DropoffAction(Facility store, Bot bot)
            : base(WorkPriority.Floor, Place.InStore(store), Place.InStore(store), bot.Cargo.Type, bot.Position, null, bot.Cargo.Count)
        {
        }

        internal DropoffAction(DropoffMemento memento, LoadContext load)
            : base(memento, load)
        {
            _handOver.Restore(memento.HandOver);
        }

        internal override BotActionMemento CreateMemento()
        {
            return WrittenHaul(new DropoffMemento { HandOver = _handOver.CreateMemento() });
        }

        internal override void OnFacilityMoved(World world, Bot bot, Facility facility)
        {
            if (Destination.IsAt(facility) && !HaulSystem.GoToDestination(world, bot, this))
            {
                HaulSystem.PutAway(world, bot);
            }
        }

        public override bool IsHandingOver(World world, Bot bot, out Facility facility, out float progress)
        {
            facility = null;
            progress = 0f;
            if (bot.IsMoving || _handOver.Total == 0 || _handOver.Remaining == _handOver.Total)
            {
                return false;
            }

            facility = HandOverFacility(world);
            if (facility == null)
            {
                return false;
            }

            progress = _handOver.Progress;
            return true;
        }

        internal override void Tick(World world, Bot bot)
        {
            DeliveryTarget target = Destination.Resolve(world)?.DeliveryFor(world, bot.Position, bot.Cargo.Count);
            if (target == null || !target.Wants(world, bot, this, out int? task))
            {
                world.Log.Write(() => bot.LogName + " の " + bot.Cargo.LogText + " は " + Destination.LogName(world) + " で要らなくなった → 片付け");
                HaulSystem.PutAway(world, bot);
                return;
            }

            Reassign(task);

            if (!BotMotion.Step(world, bot))
            {
                return;
            }

            Facility handOver = HandOverFacility(world);
            if (handOver != null)
            {
                if (_handOver.Total == 0)
                {
                    _handOver.Start(world.Configuration.TicksFor(handOver.Definition.HandOverSeconds));
                }

                if (!_handOver.IsDone)
                {
                    _handOver.CountDownOne();
                    return;
                }
            }

            HandOver(world, bot, target);
        }

        private void HandOver(World world, Bot bot, DeliveryTarget target)
        {
            if (target.Accept(world, bot) && !bot.IsCarrying)
            {
                bot.GoIdle();
                return;
            }

            HaulSystem.PutAway(world, bot);
        }

        private Facility HandOverFacility(World world)
        {
            Facility facility = world.Facilities.Find(Destination);
            return facility != null && facility.Definition.HandOverSeconds > 0f ? facility : null;
        }
    }
}
