using Scrapwalker.Core.Outpost.Bots.Haul;
using Scrapwalker.Core.Outpost.Grid;
using Scrapwalker.Core.Root;

namespace Scrapwalker.Core.Outpost
{
    internal interface IPlace
    {
        string LogName { get; }

        T Show<T>(IPlaceView<T> view);

        DeliveryTarget DeliveryFor(World world, GridPoint from, int count);
    }
}
