using System.Collections.Generic;
using Scrapwalker.Core.Items;
using Scrapwalker.Core.Outpost.Construction;
using Scrapwalker.Core.Outpost.Grid;
using Scrapwalker.Core.Root;

namespace Scrapwalker.Core.Outpost.Bots.Haul
{
    internal sealed class SiteTarget : SupplyTarget
    {
        private readonly IWorksite _site;

        public SiteTarget(IWorksite site)
        {
            _site = site;
        }

        public override GridPoint Door => _site.Door;
        public override Place Place => _site.Place;

        public override void CollectLacks(World world, Bot except, List<Lack> lacks)
        {
            lacks.Clear();
            Worksite site = _site.Site;
            foreach (MaterialAmount amount in MaterialAmount.PerMaterial(site.Cost))
            {
                int lacking = amount.Count - site.Delivered.CountOf(amount.Type) - world.CountInFlight(Place, amount.Type, null, except);
                if (lacking > 0)
                {
                    lacks.Add(new Lack(null, amount.Type, lacking, site.Filter));
                }
            }
        }

        protected override void Receive(ItemStack items)
        {
            _site.Site.Delivered.Add(items);
        }
    }
}
