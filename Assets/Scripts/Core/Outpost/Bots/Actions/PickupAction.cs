using System.Collections.Generic;
using Scrapwalker.Core.Framework;
using Scrapwalker.Core.Items;
using Scrapwalker.Core.Outpost.Bots.Haul;
using Scrapwalker.Core.Root;

namespace Scrapwalker.Core.Outpost.Bots.Actions
{
    public sealed class PickupAction : HaulAction
    {
        private readonly IPickupSource _source;

        internal PickupAction(WorkPriority priority, IPickupSource source, Place destination, MaterialDefinition type, int? task, int count)
            : base(priority, source.Place, destination, type, source.Spot, task, count)
        {
            _source = source;
        }

        internal PickupAction(PickupMemento memento, IPickupSource source, LoadContext load)
            : base(memento, load)
        {
            _source = source;
        }

        public override IEnumerable<MaterialClaim> MaterialClaims
        {
            get
            {
                yield return new MaterialClaim(Target, MaterialFlow.Take, Type, Count, Task);
                foreach (MaterialClaim claim in base.MaterialClaims)
                {
                    yield return claim;
                }
            }
        }

        internal override BotActionMemento CreateMemento()
        {
            return WrittenHaul(new PickupMemento());
        }

        internal override bool IsReachable(World world)
        {
            DeliveryTarget target = Destination.Resolve(world)?.DeliveryFor(world, Spot, Count);
            return target != null && world.Paths.CanReach(Spot, target.Door);
        }

        internal override void Tick(World world, Bot bot)
        {
            if (!BotMotion.Step(world, bot))
            {
                return;
            }

            if (!_source.TryTake(world, bot, this))
            {
                world.Log.Write(() => bot.LogName + " 拾えなかった（" + LogText(world) + "）");
                HaulSystem.PutAway(world, bot);
                return;
            }

            var dropoff = new DropoffAction(this);
            if (!HaulSystem.GoToDestination(world, bot, dropoff))
            {
                world.Log.Write(() => bot.LogName + " 拾った " + bot.Cargo.LogText + " を運ぶ先が無い → 床へ");
                HaulSystem.PutAway(world, bot);
            }
        }
    }
}
