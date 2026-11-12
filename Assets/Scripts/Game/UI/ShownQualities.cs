using System;
using System.Linq;
using Scrapwalker.Core.Items;
using Scrapwalker.Core.Root;

namespace Scrapwalker.Game.UI
{
    public readonly struct ShownQualities : IEquatable<ShownQualities>
    {
        private readonly int _bits;

        private ShownQualities(int bits)
        {
            _bits = bits;
        }

        public static ShownQualities Of(Progress progress)
        {
            int bits = 0;
            foreach (Quality q in QualityRules.All)
            {
                if (q == Quality.Plain || progress.HasObtained(q))
                {
                    bits |= 1 << (int)q;
                }
            }

            return new ShownQualities(bits);
        }

        public bool HasChoice => (_bits & ~(1 << (int)Quality.Plain)) != 0;

        public bool Contains(Quality quality)
        {
            return (_bits & (1 << (int)quality)) != 0;
        }

        public bool TryGetEnds(Quality min, Quality max, out Quality low, out Quality high)
        {
            ShownQualities self = this;
            Quality[] inRange = QualityRules.All.Where(q => q >= min && q <= max && self.Contains(q)).ToArray();
            low = inRange.FirstOrDefault();
            high = inRange.LastOrDefault();
            return inRange.Length > 0;
        }

        public bool Equals(ShownQualities other)
        {
            return _bits == other._bits;
        }

        public override bool Equals(object value)
        {
            return value is ShownQualities other && Equals(other);
        }

        public override int GetHashCode()
        {
            return _bits;
        }
    }
}
