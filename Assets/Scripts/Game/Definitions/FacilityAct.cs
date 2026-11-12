using System;
using Scrapwalker.Core.Outpost;

namespace Scrapwalker.Game.Definitions
{
    [Serializable]
    public abstract class FacilityAct
    {
        public abstract bool IsActing(Facility facility, bool isWorking, bool hasChargingBot);
    }
}
