using Scrapwalker.Core.Framework;
using Scrapwalker.Core.Outpost.Bots.Actions;
using Scrapwalker.Core.Outpost.Construction;
using Scrapwalker.Core.Outpost.Grid;
using Scrapwalker.Core.Root;

namespace Scrapwalker.Core.Outpost.Bots
{
    public abstract class BotActionMemento
    {
        public PlaceMemento Target;
        public PointMemento Spot;

        internal abstract BotAction Restore(World world, LoadContext load);

        private protected T Resolve<T>(World world, LoadContext load) where T : class
        {
            return Place.Restore(Target).Resolve(world) as T ?? throw load.Malformed();
        }
    }

    public abstract class AssignmentActionMemento : BotActionMemento
    {
        public WorkPriority Priority;
    }

    public abstract class HaulActionMemento : AssignmentActionMemento
    {
        public PlaceMemento Destination;
        public string Type;
        public int? Task;
        public int Count;
    }

    public sealed class IdleMemento : BotActionMemento
    {
        public CountdownMemento Rest;

        internal override BotAction Restore(World world, LoadContext load)
        {
            return new IdleAction(this);
        }
    }

    public sealed class PickupMemento : HaulActionMemento
    {
        internal override BotAction Restore(World world, LoadContext load)
        {
            return new PickupAction(this, Resolve<IPickupSource>(world, load), load);
        }
    }

    public sealed class DropoffMemento : HaulActionMemento
    {
        public CountdownMemento HandOver;

        internal override BotAction Restore(World world, LoadContext load)
        {
            return new DropoffAction(this, load);
        }
    }

    public sealed class FacilityWorkMemento : AssignmentActionMemento
    {
        public bool HasStartedRun;

        internal override BotAction Restore(World world, LoadContext load)
        {
            return new FacilityWorkAction(this, Resolve<Facility>(world, load));
        }
    }

    public sealed class SiteActionMemento : AssignmentActionMemento
    {
        internal override BotAction Restore(World world, LoadContext load)
        {
            return new SiteAction(this, Resolve<IWorksite>(world, load));
        }
    }

    public sealed class ChargeMemento : BotActionMemento
    {
        internal override BotAction Restore(World world, LoadContext load)
        {
            return new ChargeAction(this, Resolve<Facility>(world, load));
        }
    }
}
