using System.Collections.Generic;
using System.Linq;
using Scrapwalker.Core.Items;
using Scrapwalker.Core.Outpost.Grid;
using Scrapwalker.Core.Root;
using Scrapwalker.Core.Travel;

namespace Scrapwalker.Core.Outpost.Facilities.Storage
{
    public sealed class Stock
    {
        private readonly World _world;

        public Stock(World world)
        {
            _world = world;
            world.Events.On<Looted>(looted => StoreOrDrop(looted.Items));
        }

        public int MaterialCount => Sum(store => store.MaterialCount);

        public int Capacity => Sum(store => store.Capacity);

        public int Material(MaterialDefinition type, Quality quality)
        {
            return Sum(store => store.Material(type, quality));
        }

        public int CountMaterialInRange(MaterialDefinition type, QualityFilter filter)
        {
            return Sum(store => filter.InOrder.Sum(quality => store.Material(type, quality)));
        }

        internal void StoreOrDrop(ItemStack items)
        {
            int left = Add(items);
            if (left > 0)
            {
                _world.Floor.Drop(_world.Intake, items.WithCount(left));
            }
        }

        internal int Add(ItemStack items)
        {
            if (items.IsEmpty)
            {
                return 0;
            }

            int left = items.Count;
            foreach (Facility facility in _world.Facilities)
            {
                if (left <= 0)
                {
                    break;
                }

                if (!facility.CanStoreMore)
                {
                    continue;
                }

                int put = left < facility.Inventory.FreeSpace ? left : facility.Inventory.FreeSpace;
                facility.Inventory.Add(items.WithCount(put));
                left -= put;
            }

            return left;
        }

        public bool HasFreeSpace => _world.Facilities.Any(facility => facility.CanStoreMore);

        public Facility StoreWithFreeSpace(GridPoint from, int want)
        {
            return Nearest(facility => facility.CanStoreMore && facility.Inventory.FreeSpace >= want, from)
                ?? Nearest(facility => facility.CanStoreMore, from);
        }

        private Facility Nearest(System.Func<Facility, bool> where, GridPoint from)
        {
            return _world.Paths.Nearest(_world.Facilities, where, from, facility => facility.Door);
        }

        private IEnumerable<Inventory> Accessible()
        {
            return _world.Facilities
                .Where(facility => facility.IsStorageAccessible)
                .Select(facility => facility.Inventory);
        }

        private int Sum(System.Func<Inventory, int> count)
        {
            return Accessible().Sum(count);
        }
    }
}
