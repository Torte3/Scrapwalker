using Scrapwalker.Core.Framework;
using Scrapwalker.Core.Outpost.Bots;
using Scrapwalker.Core.Outpost.Bots.Haul;
using Scrapwalker.Core.Outpost.Construction;
using Scrapwalker.Core.Outpost.Grid;
using Scrapwalker.Core.Root;

namespace Scrapwalker.Core.Outpost.Hulls
{
    public sealed class Crack : IPlace, IWorksite
    {
        private readonly Hull _hull;

        internal Crack(Hull hull, int id, Cell cell, GameConfiguration configuration)
        {
            _hull = hull;
            ID = id;
            Cell = cell;
            Site = new Worksite(configuration.RepairCost);
        }

        public int ID { get; }
        public Cell Cell { get; }
        public Worksite Site { get; }
        public float Progress => Site.Progress;
        public GridPoint Spot => new GridPoint(Cell.Column + 0.5f, Cell.Row);
        internal bool IsRepaired { get; set; }

        internal CrackMemento CreateMemento()
        {
            return new CrackMemento { ID = ID, Cell = Cell.CreateMemento(), Site = Site.CreateMemento() };
        }

        internal static Crack Restore(Hull hull, CrackMemento memento, GameConfiguration configuration, LoadContext load)
        {
            var crack = new Crack(hull, memento.ID, Grid.Cell.Restore(memento.Cell), configuration);
            crack.Site.Restore(memento.Site, load);
            return crack;
        }

        Place IWorksite.Place => Place.AtCrack(this);
        SiteJob IWorksite.Job => SiteJob.Repair;
        float IWorksite.SupplyShare => Site.SupplyShare;
        GridPoint IWorksite.Door => Spot;
        WorkPriority IWorksite.SitePriority => WorkPriority.Repair;
        bool IWorksite.IsGone => IsRepaired;
        public string LogName => "外壁のヒビ";

        bool IWorksite.IsReady(World world)
        {
            return Site.IsSupplied;
        }

        GridPoint IWorksite.SpotFor(World world, Bot bot)
        {
            return Spot;
        }

        bool IWorksite.Advance(World world, Bot bot)
        {
            return Site.Time.Advance(bot.Speed(world.Configuration));
        }

        void IWorksite.Complete(World world)
        {
            _hull.Repair(ID);
        }

        T IPlace.Show<T>(IPlaceView<T> view)
        {
            return view.Crack();
        }

        DeliveryTarget IPlace.DeliveryFor(World world, GridPoint from, int count)
        {
            return new SiteTarget(this);
        }
    }
}
