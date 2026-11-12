using Scrapwalker.Core.Root;

namespace Scrapwalker.Core.Outpost
{
    public abstract class FacilityCommand : Command
    {
        protected FacilityCommand(int facilityID)
        {
            FacilityID = facilityID;
        }

        public int FacilityID { get; }

        internal override string LogText => base.LogText + " 部屋#" + FacilityID;

        internal sealed override CommandResult Check(World world)
        {
            Facility facility = world.Facilities.Find(FacilityID);
            return facility == null ? CommandResult.Fail(CommandError.NoFacility) : Check(world, facility);
        }

        protected sealed override void Run(World world)
        {
            Run(world, world.Facilities.Find(FacilityID));
        }

        protected abstract CommandResult Check(World world, Facility facility);

        protected abstract void Run(World world, Facility facility);
    }
}
