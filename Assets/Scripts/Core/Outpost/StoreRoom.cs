using Scrapwalker.Core.Items;
using Scrapwalker.Core.Outpost.Bots;
using Scrapwalker.Core.Outpost.Bots.Actions;
using Scrapwalker.Core.Outpost.Bots.Haul;
using Scrapwalker.Core.Outpost.Grid;
using Scrapwalker.Core.Root;

namespace Scrapwalker.Core.Outpost
{
    internal sealed class StoreRoom : IPlace, IPickupSource
    {
        private readonly Facility _store;

        public StoreRoom(Facility store)
        {
            _store = store;
            Target = new StoreTarget(store);
        }

        public StoreTarget Target { get; }
        public Place Place => Place.InStore(_store);
        public GridPoint Spot => _store.Door;
        public string LogName => _store.LogName + "の棚";

        public T Show<T>(IPlaceView<T> view) => view.Facility(_store);

        public DeliveryTarget DeliveryFor(World world, GridPoint from, int count)
        {
            return _store.Condition.IsStanding ? Target : null;
        }

        public bool TryTake(World world, Bot bot, PickupAction pickup)
        {
            if (_store.IsRemoved || !_store.IsStorageAccessible)
            {
                return false;
            }

            DeliveryTarget target = pickup.Destination.Resolve(world)?.DeliveryFor(world, bot.Position, pickup.Count);
            if (target == null || !target.TryChooseLack(world, bot, pickup, out Lack want) || !TryFindQuality(want, out Quality found))
            {
                return false;
            }

            int perTrip = target.CountToPick(world, bot, pickup, found, out int? task);
            int count = System.Math.Min(_store.Inventory.Material(want.Type, found), perTrip);
            _store.Inventory.TryTakeMaterial(want.Type, found, count);
            bot.Cargo = ItemStack.Of(want.Type, found, count);
            pickup.Reassign(task);
            pickup.Recount(count);
            return true;
        }

        private bool TryFindQuality(Lack want, out Quality found)
        {
            foreach (Quality quality in want.Filter.InOrder)
            {
                if (_store.Inventory.Material(want.Type, quality) > 0)
                {
                    found = quality;
                    return true;
                }
            }

            found = want.Filter.Min;
            return false;
        }
    }
}
