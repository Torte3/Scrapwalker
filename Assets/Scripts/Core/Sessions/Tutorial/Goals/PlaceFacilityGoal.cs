using Scrapwalker.Core.Outpost.Construction;
using Scrapwalker.Core.Root;

namespace Scrapwalker.Core.Sessions.Tutorial.Goals
{
    public sealed class PlaceFacilityGoal : FacilityGoal
    {
        internal PlaceFacilityGoal(string facility) : base(facility)
        {
        }

        internal override bool AllowsPlacing(World world, PlaceFacility place)
        {
            return place.Definition == FacilityOf(world) && world.Facilities.CountIncludingUnbuilt(place.Definition) == 0;
        }

        public override bool Reached(World world)
        {
            return world.Facilities.CountIncludingUnbuilt(FacilityOf(world)) > 0;
        }

        public override T Guide<T>(World world, ITutorialGuide<T> guide)
        {
            return guide.Place(FacilityOf(world));
        }
    }
}
