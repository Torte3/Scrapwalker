using Scrapwalker.Core.Framework;
using Scrapwalker.Core.Items;
using Scrapwalker.Core.Outpost.Bots;
using Scrapwalker.Core.Outpost.Bots.Actions;
using Scrapwalker.Core.Outpost.Bots.Haul;
using Scrapwalker.Core.Outpost.Grid;
using Scrapwalker.Core.Root;

namespace Scrapwalker.Core.Outpost.Floors
{
    public sealed class Pile : IPlace, IPickupSource
    {
        public Pile(int id, FloorSlot slot, GridPoint spot, ItemStack items)
        {
            ID = id;
            Slot = slot;
            Spot = spot;
            Items = items;
        }

        public Pile(Pile place, ItemStack items)
            : this(place.ID, place.Slot, place.Spot, items)
        {
        }

        public int ID { get; }
        public FloorSlot Slot { get; }
        public GridPoint Spot { get; }
        public ItemStack Items { get; private set; }

        public MaterialDefinition Type => Items.Type;
        public Quality Quality => Items.Quality;
        public int Count => Items.Count;

        internal PileMemento CreateMemento()
        {
            return new PileMemento { ID = ID, Slot = Slot.CreateMemento(), Spot = Spot.CreateMemento(), Items = Items.CreateMemento() };
        }

        internal static Pile Restore(PileMemento memento, LoadContext load)
        {
            return new Pile(memento.ID, FloorSlot.Restore(memento.Slot), GridPoint.Restore(memento.Spot), ItemStack.Restore(memento.Items, load));
        }

        public bool Holds(MaterialDefinition type)
        {
            return Type == type;
        }

        internal void Grow(int count)
        {
            Items = Items.WithCount(Items.Count + count);
        }

        string IPlace.LogName => "床に置いた資材";

        Place IPickupSource.Place => Place.AtPile(this);

        T IPlace.Show<T>(IPlaceView<T> view)
        {
            return view.Floor();
        }

        DeliveryTarget IPlace.DeliveryFor(World world, GridPoint from, int count)
        {
            return null;
        }

        bool IPickupSource.TryTake(World world, Bot bot, PickupAction pickup)
        {
            DeliveryTarget target = pickup.Destination.Resolve(world)?.DeliveryFor(world, bot.Position, pickup.Count);
            int count = pickup.Count;
            if (target != null)
            {
                count = target.CountToPick(world, bot, pickup, Quality, out int? task);
                pickup.Reassign(task);
            }

            if (count <= 0 || !world.Floor.TryTake(this, count, out ItemStack taken))
            {
                return false;
            }

            bot.Cargo = taken;
            pickup.Recount(taken.Count);
            return true;
        }
    }
}
