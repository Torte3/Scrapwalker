using Scrapwalker.Core.Root;

namespace Scrapwalker.Core.Travel
{
    public sealed class Depart : Command
    {
        internal override CommandResult Check(World world)
        {
            if (world.Journey.IsTraveling)
            {
                return CommandResult.Fail(CommandError.NotLanded);
            }

            return CommandResult.Success();
        }

        protected override void Run(World world)
        {
            JourneySystem.Leave(world);
        }
    }
}
