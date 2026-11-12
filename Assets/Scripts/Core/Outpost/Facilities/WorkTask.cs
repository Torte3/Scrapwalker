using Scrapwalker.Core.Framework;
using Scrapwalker.Core.Items;

namespace Scrapwalker.Core.Outpost.Facilities
{
    public sealed class WorkTask
    {
        private readonly WorkQueue _queue;
        private QualityFilter _filter = QualityFilter.Any;
        private WorkJob _heldJob;

        public int ID { get; }
        public RecipeDefinition Recipe { get; }
        public bool IsRepeating { get; internal set; }

        public QualityFilter Filter
        {
            get => _filter;
            internal set
            {
                _filter = value;
                _queue.Changed();
            }
        }

        public WorkJob HeldJob
        {
            get => _heldJob;
            internal set
            {
                _heldJob = value;
                _queue.Changed();
            }
        }

        internal WorkTask(WorkQueue queue, int id, RecipeDefinition recipe, bool repeat)
        {
            _queue = queue;
            ID = id;
            Recipe = recipe;
            IsRepeating = repeat;
        }

        internal string LogName => LogNameOf(ID);

        internal static string LogNameOf(int id)
        {
            return "タスク#" + id;
        }

        internal TaskMemento CreateMemento()
        {
            return new TaskMemento
            {
                ID = ID,
                Recipe = Recipe.ID,
                Repeat = IsRepeating,
                Filter = Filter.CreateMemento(),
                Held = HeldJob?.CreateMemento(),
            };
        }

        internal static WorkTask Restore(WorkQueue queue, TaskMemento memento, FacilityDefinition facility, LoadContext load)
        {
            RecipeDefinition recipe = facility.FindRecipe(memento.Recipe)
                ?? throw load.UnknownRecipe(memento.Recipe, facility);
            var task = new WorkTask(queue, memento.ID, recipe, memento.Repeat)
            {
                Filter = QualityFilter.Restore(memento.Filter),
            };
            task.HeldJob = memento.Held != null ? WorkJob.Restore(memento.Held, task, load) : null;
            return task;
        }
    }
}
