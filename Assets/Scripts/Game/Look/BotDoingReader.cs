using Scrapwalker.Core.Outpost;
using Scrapwalker.Core.Outpost.Bots;
using Scrapwalker.Core.Outpost.Bots.Actions;
using Scrapwalker.Core.Outpost.Construction;
using Scrapwalker.Core.Outpost.Facilities;
using Scrapwalker.Core.Outpost.Facilities.Recipes;
using Scrapwalker.Core.Root;

namespace Scrapwalker.Game.Look
{
    public sealed class BotDoingReader : IBotActionView<BotDoing>, ISiteJobView<BotVerb>, IWorkPoseView<BotVerb>
    {
        private readonly Bot _bot;

        private BotDoingReader(Bot bot)
        {
            _bot = bot;
        }

        public static BotDoing Read(Bot bot)
        {
            return bot.Action.Show(new BotDoingReader(bot));
        }

        public BotDoing Idle()
        {
            return new BotDoing(BotVerb.None, Place.Storage, Place.Storage, null, null, BotProgress.None);
        }

        public BotDoing Haul(HaulAction haul)
        {
            bool isTidy = haul.Destination == Place.Storage;
            return new BotDoing(isTidy ? BotVerb.Tidy : BotVerb.Haul, isTidy ? haul.Destination : haul.Target, haul.Destination, haul.Task, haul.Type, BotProgress.None);
        }

        public BotDoing FacilityWork(FacilityWorkAction work)
        {
            Workshop shop = work.Facility.Work;
            bool isWaiting = !_bot.IsHeading && shop.WorkingTask == null;
            BotVerb verb = isWaiting ? BotVerb.Wait : shop.CurrentTask != null ? shop.CurrentTask.Recipe.Result.ShowPose(this) : BotVerb.Make;
            BotProgress progress = !_bot.IsMoving && shop.WorkingTask != null ? new BotProgress(shop.WorkProgress, 1f) : BotProgress.None;
            return new BotDoing(verb, work.Target, work.Target, shop.CurrentTask?.ID, null, progress);
        }

        public BotDoing Site(SiteAction site)
        {
            BotProgress progress = _bot.IsMoving ? BotProgress.None : new BotProgress(site.Site.Progress, site.SupplyShare);
            return new BotDoing(site.Job.Show(this), site.Target, site.Target, null, null, progress);
        }

        public BotDoing Charge(ChargeAction charge)
        {
            return new BotDoing(BotVerb.Charge, charge.Target, charge.Target, null, null, BotProgress.None);
        }

        public BotVerb Build() => BotVerb.Build;

        public BotVerb Rebuild() => BotVerb.Rebuild;

        public BotVerb Reassemble() => BotVerb.Rebuild;

        public BotVerb Dismantle() => BotVerb.Dismantle;

        public BotVerb Repair() => BotVerb.Repair;

        public BotVerb Making() => BotVerb.Make;

        public BotVerb Firing() => BotVerb.Fire;
    }
}
