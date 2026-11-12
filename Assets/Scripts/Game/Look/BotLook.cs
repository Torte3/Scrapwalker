using Scrapwalker.Core.Outpost;
using Scrapwalker.Core.Outpost.Bots;
using Scrapwalker.Core.Root;

namespace Scrapwalker.Game.Look
{
    public static class BotLook
    {
        public static Bot WorkerInPicture(this Facility facility, World world)
        {
            if (!facility.Definition.HasStation || facility.Work.IsUpgrading || !facility.IsWorking(world))
            {
                return null;
            }

            return world.BotOperating(facility);
        }
    }
}
