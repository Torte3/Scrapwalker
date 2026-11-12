using System;
using System.Collections.Generic;

namespace Scrapwalker.Core.Framework
{
    public static class GameRandom
    {
        private static readonly Random Source = new Random();

        public static int Range(int min, int max)
        {
            return Source.Next(min, max);
        }

        public static float Value()
        {
            return (float)Source.NextDouble();
        }

        public static int PickIndex<T>(IReadOnlyList<T> items, Func<T, double> weight)
        {
            double total = 0;
            foreach (T item in items)
            {
                total += weight(item);
            }

            if (total <= 0)
            {
                return -1;
            }

            double pick = Source.NextDouble() * total;
            int last = -1;
            for (int i = 0; i < items.Count; i++)
            {
                double w = weight(items[i]);
                if (w <= 0)
                {
                    continue;
                }

                last = i;
                pick -= w;
                if (pick < 0)
                {
                    return i;
                }
            }

            return last;
        }
    }
}
