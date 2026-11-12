using System.Collections.Generic;
using Scrapwalker.Core.Outpost.Bots.Actions;
using Scrapwalker.Core.Outpost.Bots.Haul;
using Scrapwalker.Core.Root;

namespace Scrapwalker.Core.Outpost.Bots.Assignments
{
    public sealed class FacilityAssignment : Assignment
    {
        private readonly Facility _facility;
        private readonly FacilityTarget _supply;

        public FacilityAssignment(Facility facility)
        {
            _facility = facility;
            _supply = new FacilityTarget(facility);
        }

        public override IEnumerable<AssignmentAction> Actions(World world)
        {
            bool isStationed = world.StationedAt(_facility) != null;
            var work = new FacilityWorkAction(isStationed ? WorkPriority.Station : _facility.Work.CurrentTaskPriority, _facility);
            if (work.IsOpen(world))
            {
                yield return work;
            }

            WorkPriority haulPriority = isStationed ? WorkPriority.Station : WorkPriority.ProductionHaul;
            foreach (AssignmentAction haul in _supply.Hauls(world, haulPriority))
            {
                yield return haul;
            }
        }
    }
}
