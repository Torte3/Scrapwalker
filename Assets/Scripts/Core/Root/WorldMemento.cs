using System.Collections.Generic;
using Scrapwalker.Core.Outpost;
using Scrapwalker.Core.Outpost.Bots;
using Scrapwalker.Core.Outpost.Floors;
using Scrapwalker.Core.Outpost.Hulls;
using Scrapwalker.Core.Outpost.Power;
using Scrapwalker.Core.Outpost.Upgrades;
using Scrapwalker.Core.Research;
using Scrapwalker.Core.Travel;

namespace Scrapwalker.Core.Root
{
    public sealed class WorldMemento
    {
        public int Tick;
        public int NextFacilityID;
        public int NextBotID;
        public JourneyMemento Journey;
        public ExplorationMemento Exploration;
        public ProgressMemento Progress;
        public ResearchMemento Research;
        public OutpostUpgradesMemento Upgrades;
        public EnergyMemento Energy;
        public BarrierMemento Barrier;
        public HullMemento Hull;
        public List<FacilityMemento> Facilities = new List<FacilityMemento>();
        public List<PileMemento> Floor = new List<PileMemento>();
        public List<BotMemento> Bots = new List<BotMemento>();
    }
}
