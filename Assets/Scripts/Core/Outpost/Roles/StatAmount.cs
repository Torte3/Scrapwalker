using System;
using System.Collections.Generic;

namespace Scrapwalker.Core.Outpost.Roles
{
    [Serializable]
    public struct StatAmount
    {
        public FacilityStat Stat;
        public float Amount;

        public StatAmount(FacilityStat stat, float amount)
        {
            Stat = stat;
            Amount = amount;
        }

        public float Of(FacilityStat stat)
        {
            return Stat == stat ? Amount : 0f;
        }

        public static float Total(IEnumerable<StatAmount> amounts, FacilityStat stat)
        {
            float total = 0f;
            foreach (StatAmount amount in amounts)
            {
                total += amount.Of(stat);
            }

            return total;
        }
    }
}
