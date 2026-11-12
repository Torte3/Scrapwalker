using System.Collections.Generic;
using Scrapwalker.Core.Framework;
using Scrapwalker.Core.Items;
using Scrapwalker.Core.Outpost.Facilities;
using Scrapwalker.Core.Outpost.Grid;
using Scrapwalker.Core.Root;

namespace Scrapwalker.Core.Outpost.Bots.Actions
{
    public abstract class HaulAction : AssignmentAction
    {
        protected HaulAction(WorkPriority priority, Place source, Place destination, MaterialDefinition type, GridPoint spot, int? task, int count)
            : base(priority, source, spot)
        {
            Destination = destination;
            Type = type;
            Task = task;
            Count = count;
        }

        private protected HaulAction(HaulActionMemento memento, LoadContext load)
            : base(memento)
        {
            Destination = Place.Restore(memento.Destination);
            Type = load.Find<MaterialDefinition>(memento.Type);
            Task = memento.Task;
            Count = memento.Count;
        }

        public Place Destination { get; private set; }
        public MaterialDefinition Type { get; }
        public int? Task { get; private set; }
        public int Count { get; private set; }

        public override IEnumerable<MaterialClaim> MaterialClaims
        {
            get
            {
                yield return new MaterialClaim(Destination, MaterialFlow.Bring, Type, Count, Task);
            }
        }

        protected T WrittenHaul<T>(T memento) where T : HaulActionMemento
        {
            WrittenAssignment(memento);
            memento.Destination = Destination.CreateMemento();
            memento.Type = Type.ID;
            memento.Task = Task;
            memento.Count = Count;
            return memento;
        }

        public override T Show<T>(IBotActionView<T> view)
        {
            return view.Haul(this);
        }

        internal void Redirect(Place destination)
        {
            Destination = destination;
        }

        internal void Reassign(int? task)
        {
            Task = task;
        }

        internal void Recount(int count)
        {
            Count = count;
        }

        internal override string LogText(World world)
        {
            string task = Task.HasValue ? " " + WorkTask.LogNameOf(Task.Value) : string.Empty;
            return GetType().Name + "[" + Priority + "] " + Type + "×" + Count + " " + Target.LogName(world) + "→" + Destination.LogName(world) + task;
        }
    }
}
