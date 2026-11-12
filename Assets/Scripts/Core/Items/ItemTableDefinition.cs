using System;
using System.Collections.Generic;
using System.Linq;
using Scrapwalker.Core.Framework;

namespace Scrapwalker.Core.Items
{
    [Serializable]
    public sealed class ItemTableDefinition : Definition
    {
        public List<LootRange> Rows = new List<LootRange>();

        public IEnumerable<MaterialDefinition> Items => Rows.SelectMany(row => row.Entries).Select(entry => entry.Type);

        public MaterialAmount[] Roll(IEnumerable<MaterialDefinition> task)
        {
            var counts = new Dictionary<MaterialDefinition, int>();
            foreach (LootRange row in Rows)
            {
                int count = GameRandom.Range(row.Min, row.Max + 1);
                for (int i = 0; i < count; i++)
                {
                    MaterialDefinition type = row.Pick();
                    counts[type] = counts.TryGetValue(type, out int counted) ? counted + 1 : 1;
                }
            }

            var items = new List<MaterialAmount>();
            foreach (MaterialDefinition type in task)
            {
                if (counts.TryGetValue(type, out int counted))
                {
                    items.Add(new MaterialAmount(type, counted));
                }
            }

            return items.ToArray();
        }
    }
}
