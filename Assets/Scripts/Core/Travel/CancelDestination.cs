using Scrapwalker.Core.Root;

namespace Scrapwalker.Core.Travel
{
    public sealed class CancelDestination : Command
    {
        internal override CommandResult Check(World world)
        {
            Journey journey = world.Journey;
            if (!journey.IsTraveling || journey.Destination == null)
            {
                return CommandResult.Fail(CommandError.NoDestination);
            }

            return CommandResult.Success();
        }

        protected override void Run(World world)
        {
            world.Journey.DropDestination();
        }
    }
}
