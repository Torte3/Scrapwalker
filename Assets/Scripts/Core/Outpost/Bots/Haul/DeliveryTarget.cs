using Scrapwalker.Core.Items;
using Scrapwalker.Core.Outpost.Bots.Actions;
using Scrapwalker.Core.Outpost.Grid;
using Scrapwalker.Core.Root;

namespace Scrapwalker.Core.Outpost.Bots.Haul
{
    internal abstract class DeliveryTarget
    {
        public abstract Place Place { get; }

        public abstract GridPoint Door { get; }

        public abstract bool Wants(World world, Bot bot, HaulAction haul, out int? task);

        public abstract bool Accept(World world, Bot bot);

        internal virtual bool TryChooseLack(World world, Bot bot, HaulAction haul, out Lack lack)
        {
            lack = default;
            return false;
        }

        internal virtual int CountToPick(World world, Bot bot, HaulAction haul, Quality quality, out int? task)
        {
            task = haul.Task;
            return haul.Count;
        }
    }
}
