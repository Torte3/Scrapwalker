using Scrapwalker.Core.Outpost;
using Scrapwalker.Core.Outpost.Facilities;
using Scrapwalker.Game.Definitions;
using Scrapwalker.Game.MainView.Outpost;

namespace Scrapwalker.Game.MainView.Compose
{
    public struct FacilityVisual
    {
        public Facility Facility;
        public FacilityLook Look;
        public FacilityRect Rect;
        public FacilityJob Job;
        public WorkTask Task;
        public bool IsOverLimit;
        public bool IsWorking;
        public bool IsRecovering;
        public bool HasChargingBot;
        public float GaugeFill;
        public FacilityWorkerVisual? Worker;
    }
}
