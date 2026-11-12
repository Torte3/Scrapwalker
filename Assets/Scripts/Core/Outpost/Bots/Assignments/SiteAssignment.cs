using System.Collections.Generic;
using Scrapwalker.Core.Outpost.Bots.Actions;
using Scrapwalker.Core.Outpost.Bots.Haul;
using Scrapwalker.Core.Outpost.Construction;
using Scrapwalker.Core.Root;

namespace Scrapwalker.Core.Outpost.Bots.Assignments
{
    internal sealed class SiteAssignment : Assignment
    {
        private readonly IWorksite _site;
        private readonly SiteTarget _supply;

        public SiteAssignment(IWorksite site)
        {
            _site = site;
            _supply = new SiteTarget(site);
        }

        public override IEnumerable<AssignmentAction> Actions(World world)
        {
            var work = new SiteAction(_site);
            if (work.IsValid(world))
            {
                if (work.IsFree(world))
                {
                    yield return work;
                }

                yield break;
            }

            foreach (AssignmentAction haul in _supply.Hauls(world, _site.SitePriority))
            {
                yield return haul;
            }
        }
    }
}
