using Scrapwalker.Core.Outpost.Bots;
using Scrapwalker.Core.Outpost.Bots.Actions;
using Scrapwalker.Core.Outpost.Grid;
using Scrapwalker.Core.Root;

namespace Scrapwalker.Core.Outpost
{
    internal interface IPickupSource
    {
        Place Place { get; }

        GridPoint Spot { get; }

        bool TryTake(World world, Bot bot, PickupAction pickup);
    }
}
