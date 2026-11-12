using Scrapwalker.Core.Outpost.Construction;
using Scrapwalker.Core.Outpost.Facilities.Commands;
using Scrapwalker.Core.Travel;

namespace Scrapwalker.Core.Root
{
    public class CommandRule
    {
        public static readonly CommandRule All = new CommandRule();

        internal virtual CommandError Refusal => CommandError.None;

        internal virtual bool AllowsOther(World world, Command command) => true;

        internal virtual bool AllowsPlacing(World world, PlaceFacility place) => AllowsOther(world, place);
        internal virtual bool AllowsTask(World world, AddWorkTask task) => AllowsOther(world, task);
        internal virtual bool AllowsTravel(World world, TravelTo travel) => AllowsOther(world, travel);
        internal virtual bool AllowsLockOn(World world, Command lockOn) => AllowsOther(world, lockOn);

        internal virtual bool AllowsSaving => true;
    }
}
