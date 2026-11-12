using System;
using System.Collections.Generic;

namespace Scrapwalker.Core.Items
{
    public readonly struct QualityFilter : IEquatable<QualityFilter>
    {
        public static readonly QualityFilter Any = new QualityFilter(Quality.Plain, Quality.Best, false);

        public static QualityFilter Exactly(Quality quality)
        {
            return new QualityFilter(quality, quality, false);
        }

        public QualityFilter(Quality min, Quality max, bool highFirst)
        {
            Min = min;
            Max = max;
            IsHighFirst = highFirst;
        }

        public Quality Min { get; }
        public Quality Max { get; }
        public bool IsHighFirst { get; }

        public bool IsValid => Min <= Max;

        public int Count => Math.Max(0, (int)Max - (int)Min + 1);

        internal string LogText => Min + "〜" + Max + (IsHighFirst ? " 高い方から" : " 低い方から");

        internal FilterMemento CreateMemento()
        {
            return new FilterMemento { Min = Min, Max = Max, HighFirst = IsHighFirst };
        }

        internal static QualityFilter Restore(FilterMemento memento)
        {
            return new QualityFilter(memento.Min, memento.Max, memento.HighFirst);
        }

        public bool Accepts(Quality quality)
        {
            return quality >= Min && quality <= Max;
        }

        public Quality At(int i)
        {
            return (Quality)(IsHighFirst ? (int)Max - i : (int)Min + i);
        }

        public IEnumerable<Quality> InOrder
        {
            get
            {
                for (int i = 0; i < Count; i++)
                {
                    yield return At(i);
                }
            }
        }

        public bool Prefers(Quality a, Quality b)
        {
            return IsHighFirst ? a > b : a < b;
        }

        public QualityFilter WithRange(Quality min, Quality max)
        {
            return new QualityFilter(min, max, IsHighFirst);
        }

        public QualityFilter WithHighFirst(bool highFirst)
        {
            return new QualityFilter(Min, Max, highFirst);
        }

        public bool Equals(QualityFilter other)
        {
            return Min == other.Min && Max == other.Max && IsHighFirst == other.IsHighFirst;
        }

        public override bool Equals(object value)
        {
            return value is QualityFilter other && Equals(other);
        }

        public override int GetHashCode()
        {
            return ((int)Min * 8 + (int)Max) * 2 + (IsHighFirst ? 1 : 0);
        }

        public override string ToString()
        {
            return Min + "-" + Max + (IsHighFirst ? " high" : string.Empty);
        }
    }
}
