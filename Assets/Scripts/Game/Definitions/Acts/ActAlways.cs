using System;
using Scrapwalker.Core.Outpost;

namespace Scrapwalker.Game.Definitions.Acts
{
    [Serializable]
    public sealed class ActAlways : FacilityAct
    {
        public override bool IsActing(Facility facility, bool isWorking, bool hasChargingBot)
        {
            return true;
        }
    }
}
