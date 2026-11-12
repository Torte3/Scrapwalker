using System.Collections.Generic;
using Scrapwalker.Core.Items;

namespace Scrapwalker.Core.Outpost.Bots.Haul
{
    public readonly struct Lack
    {
        public readonly int? Task;
        public readonly MaterialDefinition Type;
        public readonly int Count;
        public readonly QualityFilter Filter;

        public Lack(int? task, MaterialDefinition type, int count, QualityFilter filter)
        {
            Task = task;
            Type = type;
            Count = count;
            Filter = filter;
        }

        public static int PerTrip(IReadOnlyList<Lack> lacks, int requested, Quality quality)
        {
            Lack first = lacks[requested];
            int count = first.Count;
            for (int i = 0; i < lacks.Count; i++)
            {
                if (i != requested && lacks[i].Accepts(first.Type, quality))
                {
                    count += lacks[i].Count;
                }
            }

            return System.Math.Min(count, first.Type.StackLimit);
        }

        public bool Accepts(MaterialDefinition type, Quality quality)
        {
            return Type == type && Filter.Accepts(quality);
        }
    }
}
