using Scrapwalker.Core.Outpost.Bots;
using Scrapwalker.Core.Outpost.Grid;
using Scrapwalker.Core.Root;

namespace Scrapwalker.Core.Outpost.Construction
{
    internal interface IWorksite
    {
        Worksite Site { get; }

        Place Place { get; }

        SiteJob Job { get; }

        float SupplyShare { get; }

        GridPoint Door { get; }

        WorkPriority SitePriority { get; }

        string LogName { get; }

        bool IsGone { get; }

        bool IsReady(World world);

        GridPoint SpotFor(World world, Bot bot);

        bool Advance(World world, Bot bot);

        void Complete(World world);
    }
}
