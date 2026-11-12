using Scrapwalker.Core.Outpost.Grid;

namespace Scrapwalker.Core.Outpost
{
    public sealed class ConditionMemento
    {
        public FacilityStage Stage;
        public CellMemento Target;

        internal FacilityCondition Restore()
        {
            return FacilityCondition.Of(Stage, Target != null ? Cell.Restore(Target) : (Cell?)null);
        }
    }
}
