using Scrapwalker.Core.Outpost.Construction;
using Scrapwalker.Core.Outpost.Grid;
using Scrapwalker.Core.Root;

namespace Scrapwalker.Core.Outpost.Bots.Actions
{
    public sealed class SiteAction : PostAction
    {
        private readonly IWorksite _site;

        internal SiteAction(IWorksite site)
            : base(site.SitePriority, site.Place, site.Door)
        {
            _site = site;
        }

        internal SiteAction(SiteActionMemento memento, IWorksite site)
            : base(memento)
        {
            _site = site;
        }

        public Worksite Site => _site.Site;
        public SiteJob Job => _site.Job;
        public float SupplyShare => _site.SupplyShare;

        internal override BotActionMemento CreateMemento()
        {
            return WrittenAssignment(new SiteActionMemento());
        }

        public override T Show<T>(IBotActionView<T> view)
        {
            return view.Site(this);
        }

        internal override GridPoint SpotFor(World world, Bot bot)
        {
            return _site.SpotFor(world, bot);
        }

        internal override bool IsValid(World world)
        {
            return !_site.IsGone && _site.IsReady(world);
        }

        protected override void Work(World world, Bot bot)
        {
            if (!_site.Advance(world, bot))
            {
                return;
            }

            world.Log.Write(() => bot.LogName + " が " + _site.LogName + " の" + _site.Job + "を終えた");
            _site.Complete(world);
            bot.GoIdle();
        }
    }
}
