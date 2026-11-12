using Scrapwalker.Core.Root;

namespace Scrapwalker.Core.Outpost.Grid
{
    public static class GridRules
    {
        public static CommandError CheckPlacement(World world, CellRect rect, Facility moving = null)
        {
            if (!rect.FitsIn(world.Columns, world.Rows))
            {
                return CommandError.OutOfGrid;
            }

            foreach (Facility f in world.Facilities)
            {
                if (f == moving)
                {
                    continue;
                }

                if (f.Rect.Overlaps(rect))
                {
                    return CommandError.Overlaps;
                }

                CellRect? relocation = f.RelocateRect;
                if (relocation.HasValue && relocation.Value.Overlaps(rect))
                {
                    return CommandError.OverlapsRelocation;
                }
            }

            return CommandError.None;
        }
    }
}
