using System.Linq;
using Scrapwalker.Core.Items;
using Scrapwalker.Core.Outpost;
using Scrapwalker.Core.Outpost.Facilities;
using Scrapwalker.Core.Research;
using Scrapwalker.Core.Root;
using Scrapwalker.Game.Look.Art;
using UnityEngine;

namespace Scrapwalker.Game.Definitions
{
    [CreateAssetMenu(fileName = "Research", menuName = "Scrapwalker/Research")]
    public sealed class ResearchAsset : DefinitionAsset<ResearchDefinition>
    {
        [SerializeField] private string _name;
        [SerializeField] private ArtEntry _picture;

        public override string Name => _name;
        public ArtEntry Picture => _picture;

        public FacilityDefinition Facility(GameDefinitions definitions)
        {
            return definitions.FacilitiesUnlockedBy(Value).FirstOrDefault();
        }
    }
}
