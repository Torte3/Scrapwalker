using Scrapwalker.Core.Outpost.Bots.Actions;

namespace Scrapwalker.Core.Outpost.Bots
{
    public interface IBotActionView<out T>
    {
        T Idle();

        T Haul(HaulAction haul);

        T FacilityWork(FacilityWorkAction work);

        T Site(SiteAction site);

        T Charge(ChargeAction charge);
    }
}
