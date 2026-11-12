using Scrapwalker.Core.Items;
using Scrapwalker.Core.Outpost.Bots.Actions;
using Scrapwalker.Core.Outpost.Grid;
using Scrapwalker.Core.Root;

namespace Scrapwalker.Core.Outpost.Bots.Haul
{
    internal sealed class StoreTarget : DeliveryTarget
    {
        private readonly Facility _store;

        public StoreTarget(Facility store)
        {
            _store = store;
        }

        public override Place Place => Place.InStore(_store);
        public override GridPoint Door => _store.Door;

        public override bool Wants(World world, Bot bot, HaulAction haul, out int? task)
        {
            task = haul.Task;
            return _store.IsOperational;
        }

        public override bool Accept(World world, Bot bot)
        {
            if (!_store.IsOperational)
            {
                return false;
            }

            ItemStack cargo = bot.Cargo;
            int put = System.Math.Min(cargo.Count, _store.Inventory.FreeSpace);
            if (put <= 0)
            {
                return false;
            }

            _store.Inventory.Add(cargo.WithCount(put));
            bot.Cargo = cargo.WithCount(cargo.Count - put);
            return true;
        }
    }
}
