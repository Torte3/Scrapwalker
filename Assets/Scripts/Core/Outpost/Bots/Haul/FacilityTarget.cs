using System.Collections.Generic;
using System.Linq;
using Scrapwalker.Core.Items;
using Scrapwalker.Core.Outpost.Facilities;
using Scrapwalker.Core.Outpost.Facilities.Recipes;
using Scrapwalker.Core.Outpost.Grid;
using Scrapwalker.Core.Root;

namespace Scrapwalker.Core.Outpost.Bots.Haul
{
    internal sealed class FacilityTarget : SupplyTarget
    {
        private readonly Facility _facility;
        private readonly List<InputShare> _shares = new List<InputShare>();
        private readonly List<int> _lacking = new List<int>();
        private readonly List<Leftover> _leftovers = new List<Leftover>();

        public FacilityTarget(Facility facility)
        {
            _facility = facility;
        }

        public override GridPoint Door => _facility.Door;
        public override Place Place => Place.At(_facility);
        protected override bool IsOpen => _facility.IsOperational;

        public override void CollectLacks(World world, Bot except, List<Lack> lacks)
        {
            _shares.Clear();
            _shares.AddRange(_facility.Work.Allocate().Shares);
            _lacking.Clear();
            foreach (InputShare share in _shares)
            {
                _lacking.Add(share.Need.Count - share.Fed);
            }

            _leftovers.Clear();
            foreach (Bot bot in world.Bots)
            {
                if (bot != except)
                {
                    CountOwnShare(bot);
                }
            }

            foreach (Leftover leftover in _leftovers)
            {
                CountLeftover(leftover);
            }

            lacks.Clear();
            for (int i = 0; i < _shares.Count; i++)
            {
                if (_lacking[i] > 0)
                {
                    InputShare share = _shares[i];
                    lacks.Add(new Lack(share.Task.ID, share.Need.Type, _lacking[i], share.Task.Filter));
                }
            }
        }

        private void CountOwnShare(Bot bot)
        {
            MaterialDefinition type = CarriedType(bot);
            if (type == null)
            {
                return;
            }

            int count = bot.Action.Bringing(Place, type, null);
            int left = count - FillOwnRow(bot, type, count);
            if (left > 0)
            {
                _leftovers.Add(new Leftover(bot, type, left));
            }
        }

        private MaterialDefinition CarriedType(Bot bot)
        {
            foreach (InputShare share in _shares)
            {
                if (bot.Action.Bringing(Place, share.Need.Type, null) > 0)
                {
                    return share.Need.Type;
                }
            }

            return null;
        }

        private int FillOwnRow(Bot bot, MaterialDefinition type, int count)
        {
            for (int i = 0; i < _shares.Count; i++)
            {
                if (_shares[i].Need.Type == type && bot.Action.Bringing(Place, type, _shares[i].Task.ID) > 0)
                {
                    return Fill(i, count);
                }
            }

            return 0;
        }

        private void CountLeftover(Leftover leftover)
        {
            bool known = leftover.Bot.IsCarrying && leftover.Bot.Cargo.Type == leftover.Type;
            int left = leftover.Count;
            for (int i = 0; i < _shares.Count && left > 0; i++)
            {
                InputShare share = _shares[i];
                bool accepts = !known || share.Task.Filter.Accepts(leftover.Bot.Cargo.Quality);
                if (share.Need.Type == leftover.Type && accepts)
                {
                    left -= Fill(i, left);
                }
            }
        }

        private int Fill(int index, int count)
        {
            int filled = System.Math.Min(count, System.Math.Max(0, _lacking[index]));
            _lacking[index] -= filled;
            return filled;
        }

        private readonly struct Leftover
        {
            public readonly Bot Bot;
            public readonly MaterialDefinition Type;
            public readonly int Count;

            public Leftover(Bot bot, MaterialDefinition type, int count)
            {
                Bot = bot;
                Type = type;
                Count = count;
            }
        }

        protected override void Receive(ItemStack items)
        {
            _facility.Work.Inputs.Add(items);
        }
    }
}
