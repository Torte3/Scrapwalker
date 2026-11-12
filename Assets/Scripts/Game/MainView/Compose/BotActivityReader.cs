using Scrapwalker.Core.Outpost;
using Scrapwalker.Core.Outpost.Bots;
using Scrapwalker.Core.Outpost.Bots.Actions;
using Scrapwalker.Core.Outpost.Construction;

namespace Scrapwalker.Game.MainView.Compose
{
    internal sealed class BotActivityReader : IBotActionView<BotActivity>, ISiteJobView<BotActivity>
    {
        private FrameVisual _frame;
        private Bot _bot;

        public BotActivity Read(FrameVisual frame, Bot bot)
        {
            _frame = frame;
            _bot = bot;
            return bot.IsMoving ? BotActivity.None : bot.Action.Show(this);
        }

        public BotActivity Idle() => BotActivity.None;

        public BotActivity Haul(HaulAction haul) => BotActivity.None;

        public BotActivity FacilityWork(FacilityWorkAction work)
        {
            Facility facility = work.Facility;
            if (_frame.TryGetFacility(facility.ID, out FacilityVisual shown) && shown.Worker.HasValue && shown.Worker.Value.Bot == _bot)
            {
                return BotActivity.InPicture(shown.Worker.Value.Stand);
            }

            return facility.Work.IsUpgrading && facility.IsStaffed ? BotActivity.Welding : BotActivity.None;
        }

        public BotActivity Site(SiteAction site) => site.Job.Show(this);

        public BotActivity Charge(ChargeAction charge)
        {
            return charge.Charger != null ? BotActivity.Charging(new FacilityRect(charge.Charger.Rect)) : BotActivity.ChargingAway;
        }

        public BotActivity Build() => BotActivity.Welding;

        public BotActivity Rebuild() => BotActivity.Welding;

        public BotActivity Reassemble() => BotActivity.Welding;

        public BotActivity Dismantle() => BotActivity.Welding;

        public BotActivity Repair() => BotActivity.Repairing;
    }
}
