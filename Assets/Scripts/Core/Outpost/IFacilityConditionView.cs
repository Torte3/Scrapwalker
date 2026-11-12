using Scrapwalker.Core.Outpost.Grid;

namespace Scrapwalker.Core.Outpost
{
    public interface IFacilityConditionView<out T>
    {
        T Planned();

        T Rebuilding();

        T Reassembling();

        T Operational();

        T Dismantling();

        T Relocating(Cell target);
    }
}
