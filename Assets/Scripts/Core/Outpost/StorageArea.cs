using Scrapwalker.Core.Outpost.Bots.Haul;
using Scrapwalker.Core.Outpost.Grid;
using Scrapwalker.Core.Root;

namespace Scrapwalker.Core.Outpost
{
    internal sealed class StorageArea : IPlace
    {
        public static readonly StorageArea Instance = new StorageArea();

        public string LogName => "資材庫";

        public T Show<T>(IPlaceView<T> view) => view.Storage();

        public DeliveryTarget DeliveryFor(World world, GridPoint from, int count)
        {
            Facility nearest = world.Stock.StoreWithFreeSpace(from, count);
            return nearest?.StoreRoom.Target;
        }
    }
}
