using System;
using System.Collections.Generic;
using Scrapwalker.Core.Framework;

namespace Scrapwalker.Core.Items
{
    [Serializable]
    public sealed class LootRange
    {
        public int Min;
        public int Max;
        public List<TableEntry> Entries = new List<TableEntry>();

        public int TotalWeight
        {
            get
            {
                int total = 0;
                foreach (TableEntry entry in Entries)
                {
                    total += entry.Weight;
                }

                return total;
            }
        }

        public MaterialDefinition Pick()
        {
            int index = GameRandom.PickIndex(Entries, e => e.Weight);
            return index >= 0 ? Entries[index].Type : throw new InvalidOperationException("落とし物の行の重みの合計が 0");
        }
    }
}
