using System.Collections.Generic;
using Scrapwalker.Core.Framework;
using Scrapwalker.Core.Items;
using Scrapwalker.Core.Outpost.Upgrades;
using Scrapwalker.Core.Research;
using Scrapwalker.Core.Travel;

namespace Scrapwalker.Core.Outpost
{
    public sealed class HullBreached : GameEvent
    {
    }

    public sealed class MaterialsMade : GameEvent
    {
        public MaterialsMade(Facility facility, ItemStack items)
        {
            FacilityID = facility.ID;
            Items = items;
        }

        public int FacilityID { get; }
        public ItemStack Items { get; }
    }

    public sealed class ResearchWorkFinished : GameEvent
    {
        public ResearchWorkFinished(ResearchDefinition research)
        {
            Research = research;
        }

        public ResearchDefinition Research { get; }
    }

    public sealed class FacilityRemoved : GameEvent
    {
        public FacilityRemoved(Facility facility)
        {
            FacilityID = facility.ID;
        }

        public int FacilityID { get; }
    }

    public sealed class OutpostUpgraded : GameEvent
    {
        public OutpostUpgraded(OutpostUpgradeDefinition upgrade, IReadOnlyList<RuinDefinition> ruins)
        {
            Upgrade = upgrade;
            Ruins = ruins;
        }

        public OutpostUpgradeDefinition Upgrade { get; }
        public IReadOnlyList<RuinDefinition> Ruins { get; }
        public int Found => Ruins.Count;
    }
}
