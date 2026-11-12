using System;
using Scrapwalker.Core.Outpost;

namespace Scrapwalker.Game.Definitions
{
    [Serializable]
    public abstract class FacilityGauge
    {
        public static readonly FacilityGauge None = new NoGauge();

        public virtual int CountStored(Facility facility)
        {
            return 0;
        }

        public abstract float Step(int stored, float fill, bool isActing);

        private sealed class NoGauge : FacilityGauge
        {
            public override float Step(int stored, float fill, bool isActing)
            {
                return 0f;
            }
        }
    }
}
