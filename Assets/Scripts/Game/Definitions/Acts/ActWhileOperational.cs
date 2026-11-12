using System;
using Scrapwalker.Core.Outpost;

namespace Scrapwalker.Game.Definitions.Acts
{
    [Serializable]
    public sealed class ActWhileOperational : FacilityAct
    {
        public override bool IsActing(Facility facility, bool isWorking, bool hasChargingBot)
        {
            return facility.IsOperational;
        }
    }
}
