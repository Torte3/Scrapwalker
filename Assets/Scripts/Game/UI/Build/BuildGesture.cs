using Scrapwalker.Core.Items;
using Scrapwalker.Core.Outpost;
using Scrapwalker.Core.Outpost.Grid;
using Scrapwalker.Core.Root;

namespace Scrapwalker.Game.UI.Build
{
    public abstract class BuildGesture
    {
        public static BuildGesture None { get; } = new NoGesture();

        public abstract FacilityDefinition Definition { get; }
        public virtual FacilityDefinition PlacingDefinition => null;
        public virtual Facility Relocating => null;
        internal abstract string ClickText { get; }

        public virtual bool EndsOnDeselect => false;

        public virtual bool DeselectsAfterRun => false;

        internal abstract Command CommandAt(Cell at, QualityFilter filter);

        internal virtual bool ContinuesAfterRun(bool keepPlacing)
        {
            return false;
        }

        private sealed class NoGesture : BuildGesture
        {
            public override FacilityDefinition Definition => null;
            internal override string ClickText => string.Empty;

            internal override Command CommandAt(Cell at, QualityFilter filter)
            {
                return null;
            }
        }
    }
}
