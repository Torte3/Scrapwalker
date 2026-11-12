using System;
using Scrapwalker.Core.Root;

namespace Scrapwalker.Core.Outpost.Upgrades
{
    [Serializable]
    public sealed class TimeOfDayEffect : UpgradeEffect
    {
        public TimeOfDay Time;

        internal override void ApplyTo(OutpostTraits traits)
        {
            traits.TimeOfDay = Time > traits.TimeOfDay ? Time : traits.TimeOfDay;
        }
    }
}
