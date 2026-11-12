using System.Linq;
using Scrapwalker.Core.Outpost.Grid;
using Scrapwalker.Core.Root;

namespace Scrapwalker.Core.Outpost.Bots
{
    public abstract class AssignmentAction : BotAction
    {
        protected AssignmentAction(WorkPriority priority, Place target, GridPoint spot)
            : base(target, spot)
        {
            Priority = priority;
        }

        protected AssignmentAction(AssignmentActionMemento memento)
            : base(memento)
        {
            Priority = memento.Priority;
        }

        public WorkPriority Priority { get; }

        protected T WrittenAssignment<T>(T memento) where T : AssignmentActionMemento
        {
            Written(memento).Priority = Priority;
            return memento;
        }

        internal bool Serves(Facility facility)
        {
            return PlaceClaims.Any(claim => claim.Place.IsAt(facility))
                || MaterialClaims.Any(claim => claim.Flow == MaterialFlow.Bring && claim.Place.IsAt(facility));
        }

        internal virtual bool IsReachable(World world)
        {
            return true;
        }

        internal override string LogText(World world)
        {
            return GetType().Name + "[" + Priority + "] " + Target.LogName(world);
        }
    }
}
