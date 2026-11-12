using System.Collections.Generic;
using Scrapwalker.Core.Items;
using Scrapwalker.Core.Outpost.Construction;
using Scrapwalker.Core.Outpost.Facilities;
using Scrapwalker.Core.Outpost.Grid;

namespace Scrapwalker.Core.Outpost
{
    public sealed class FacilityMemento
    {
        public int ID;
        public string Facility;
        public CellMemento Cell;
        public ConditionMemento Condition;
        public int HP;
        public Quality? Quality;
        public bool Staffed;
        public float StaffSpeed;
        public StationingMemento Stationing;
        public WorksiteMemento Site;
        public WorkshopMemento Work;
        public List<StackMemento> Inventory;
    }
}
