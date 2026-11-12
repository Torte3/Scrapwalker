using System.Collections.Generic;
using Scrapwalker.Core.Framework;
using Scrapwalker.Core.Items;
using Scrapwalker.Core.Outpost.Floors;
using Scrapwalker.Core.Outpost.Grid;
using Scrapwalker.Core.Root;

namespace Scrapwalker.Core.Outpost.Construction
{
    public sealed class Worksite
    {
        public Worksite(IReadOnlyList<MaterialAmount> cost)
        {
            Cost = cost;
        }

        public IReadOnlyList<MaterialAmount> Cost { get; }
        public StackList Delivered { get; } = new StackList();
        public QualityFilter Filter { get; internal set; } = QualityFilter.Any;
        public Countdown Time { get; } = new Countdown();

        public float Progress => Time.Progress;
        public bool IsSupplied => Delivered.Covers(Cost);
        public int Needed => MaterialAmount.Total(Cost);

        public float SupplyShare => Needed == 0 ? 0f : System.Math.Min(1f, (float)Delivered.Count / Needed);

        internal WorksiteMemento CreateMemento()
        {
            return new WorksiteMemento
            {
                Delivered = Delivered.CreateMemento(),
                Filter = Filter.CreateMemento(),
                Time = Time.CreateMemento(),
            };
        }

        internal void Restore(WorksiteMemento memento, LoadContext load)
        {
            Delivered.Restore(memento.Delivered, load);
            Filter = QualityFilter.Restore(memento.Filter);
            Time.Restore(memento.Time);
        }

        internal void Spill(World world, GridPoint at)
        {
            SpillSystem.Empty(world, Delivered, at);
        }
    }
}
