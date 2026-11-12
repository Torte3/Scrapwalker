using Scrapwalker.Core.Outpost;
using Scrapwalker.Core.Outpost.Grid;

namespace Scrapwalker.Game.Look
{
    public readonly struct BuildGhost
    {
        public BuildGhost(FacilityDefinition definition, Cell at, bool isPlaceable, string note)
        {
            Definition = definition;
            At = at;
            IsPlaceable = isPlaceable;
            Note = note ?? string.Empty;
        }

        public FacilityDefinition Definition { get; }
        public Cell At { get; }
        public bool IsPlaceable { get; }
        public string Note { get; }
        public CellRect Rect => Definition.RectAt(At);
    }
}
