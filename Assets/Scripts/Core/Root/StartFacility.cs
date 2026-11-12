using Scrapwalker.Core.Framework;
using Scrapwalker.Core.Outpost;
using Scrapwalker.Core.Outpost.Grid;

namespace Scrapwalker.Core.Root
{
    [System.Serializable]
    public struct StartFacility
    {
        public DefinitionReference<FacilityDefinition> FacilityReference;
        public int Column;
        public int Row;

        public FacilityDefinition Facility
        {
            get => FacilityReference?.Definition;
            set => FacilityReference = new DefinitionReference<FacilityDefinition>(value);
        }

        public Cell Cell => new Cell(Column, Row);
    }
}
