using System.Collections.Generic;
using Scrapwalker.Core.Items;
using Scrapwalker.Core.Outpost.Bots.Actions;
using Scrapwalker.Core.Root;

namespace Scrapwalker.Core.Outpost.Bots.Haul
{
    internal abstract class SupplyTarget : DeliveryTarget
    {
        private readonly List<Lack> _lacks = new List<Lack>();

        protected virtual bool IsOpen => true;

        public abstract void CollectLacks(World world, Bot except, List<Lack> lacks);

        protected abstract void Receive(ItemStack items);

        public IEnumerable<AssignmentAction> Hauls(World world, WorkPriority priority)
        {
            if (!IsOpen)
            {
                yield break;
            }

            CollectLacks(world, null, _lacks);
            for (int i = 0; i < _lacks.Count; i++)
            {
                AssignmentAction haul = world.Ledger.HaulFor(priority, _lacks, i, Place, Door);
                if (haul != null)
                {
                    yield return haul;
                }
            }
        }

        public override bool Wants(World world, Bot bot, HaulAction haul, out int? task)
        {
            task = haul.Task;
            if (!IsOpen || !bot.IsCarrying)
            {
                return false;
            }

            CollectLacks(world, bot, _lacks);
            foreach (Lack lack in _lacks)
            {
                if (lack.Accepts(bot.Cargo.Type, bot.Cargo.Quality))
                {
                    task = lack.Task;
                    return true;
                }
            }

            return false;
        }

        internal override bool TryChooseLack(World world, Bot bot, HaulAction haul, out Lack lack)
        {
            lack = default;
            if (!IsOpen)
            {
                return false;
            }

            CollectLacks(world, bot, _lacks);
            int requested = IndexOfRequested(haul, null);
            if (requested < 0)
            {
                return false;
            }

            lack = _lacks[requested];
            return true;
        }

        internal override int CountToPick(World world, Bot bot, HaulAction haul, Quality quality, out int? task)
        {
            task = haul.Task;
            if (!IsOpen)
            {
                return 0;
            }

            CollectLacks(world, bot, _lacks);
            int requested = IndexOfRequested(haul, quality);
            if (requested < 0)
            {
                return 0;
            }

            task = _lacks[requested].Task;
            return Lack.PerTrip(_lacks, requested, quality);
        }

        private int IndexOfRequested(HaulAction haul, Quality? quality)
        {
            int chosen = -1;
            for (int i = 0; i < _lacks.Count; i++)
            {
                Lack lack = _lacks[i];
                if (lack.Type != haul.Type || (quality.HasValue && !lack.Filter.Accepts(quality.Value)))
                {
                    continue;
                }

                if (lack.Task == haul.Task)
                {
                    return i;
                }

                if (chosen < 0)
                {
                    chosen = i;
                }
            }

            return chosen;
        }

        public override bool Accept(World world, Bot bot)
        {
            if (!IsOpen || !bot.IsCarrying)
            {
                return false;
            }

            CollectLacks(world, bot, _lacks);
            int wanted = 0;
            foreach (Lack lack in _lacks)
            {
                if (lack.Accepts(bot.Cargo.Type, bot.Cargo.Quality))
                {
                    wanted += lack.Count;
                }
            }

            int put = System.Math.Min(bot.Cargo.Count, wanted);
            if (put <= 0)
            {
                return false;
            }

            Receive(bot.Cargo.WithCount(put));
            bot.Cargo = bot.Cargo.WithCount(bot.Cargo.Count - put);
            return true;
        }
    }
}
