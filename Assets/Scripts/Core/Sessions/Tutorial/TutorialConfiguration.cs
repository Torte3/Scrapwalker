using System.Collections.Generic;
using Scrapwalker.Core.Outpost.Upgrades;
using Scrapwalker.Core.Root;

namespace Scrapwalker.Core.Sessions.Tutorial
{
    [System.Serializable]
    public sealed class TutorialConfiguration
    {
        public List<StartFacility> Wrecks = new List<StartFacility>();
        public float HoldSeconds;
        public float WakeSeconds;
        public float PanSeconds;

        public OutpostUpgradeDefinition RadarRepair { get; set; }
    }
}
