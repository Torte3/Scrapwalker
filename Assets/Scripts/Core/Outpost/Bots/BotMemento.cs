using System.Collections.Generic;
using Scrapwalker.Core.Items;
using Scrapwalker.Core.Outpost.Grid;

namespace Scrapwalker.Core.Outpost.Bots
{
    public sealed class BotMemento
    {
        public int ID;
        public float BatterySeconds;
        public PointMemento Position;
        public BotActionMemento Action;
        public PointMemento Goal;
        public List<PointMemento> Path = new List<PointMemento>();
        public int PathIndex;
        public StackMemento Cargo;
        public bool Lost;
        public int? StationID;
    }
}
