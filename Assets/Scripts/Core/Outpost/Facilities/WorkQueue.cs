using System.Collections.Generic;
using System.Linq;
using Scrapwalker.Core.Framework;

namespace Scrapwalker.Core.Outpost.Facilities
{
    public sealed class WorkQueue
    {
        private readonly List<WorkTask> _tasks = new List<WorkTask>();
        private readonly Dictionary<WorkTask, int> _positions = new Dictionary<WorkTask, int>();
        private readonly Dictionary<int, WorkTask> _byID = new Dictionary<int, WorkTask>();
        private int _indexedVersion = -1;

        public IReadOnlyList<WorkTask> Tasks => _tasks;
        public int NextTaskID { get; internal set; } = 1;
        public int Count => _tasks.Count;
        public WorkTask First => _tasks.Count > 0 ? _tasks[0] : null;
        internal int Version { get; private set; }

        public int IndexOf(WorkTask task)
        {
            Index();
            return _positions.TryGetValue(task, out int position) ? position : -1;
        }

        public bool Contains(RecipeDefinition recipe)
        {
            return _tasks.Exists(o => o.Recipe == recipe);
        }

        public WorkTask Find(int taskID)
        {
            Index();
            return _byID.TryGetValue(taskID, out WorkTask task) ? task : null;
        }

        internal void Changed()
        {
            Version++;
        }

        internal WorkTask Add(RecipeDefinition recipe, bool repeat)
        {
            var task = new WorkTask(this, NextTaskID++, recipe, repeat);
            _tasks.Add(task);
            Changed();
            return task;
        }

        internal void Finish(WorkTask task)
        {
            _tasks.Remove(task);
            if (task.IsRepeating)
            {
                _tasks.Add(task);
            }

            Changed();
        }

        internal void Remove(WorkTask task)
        {
            _tasks.Remove(task);
            Changed();
        }

        internal void Move(int from, int to)
        {
            WorkTask task = _tasks[from];
            _tasks.RemoveAt(from);
            _tasks.Insert(to, task);
            Changed();
        }

        private void Index()
        {
            if (_indexedVersion == Version)
            {
                return;
            }

            _positions.Clear();
            _byID.Clear();
            for (int i = 0; i < _tasks.Count; i++)
            {
                WorkTask task = _tasks[i];
                if (!_positions.ContainsKey(task))
                {
                    _positions.Add(task, i);
                }

                if (!_byID.ContainsKey(task.ID))
                {
                    _byID.Add(task.ID, task);
                }
            }

            _indexedVersion = Version;
        }

        internal QueueMemento CreateMemento()
        {
            return new QueueMemento
            {
                NextTaskID = NextTaskID,
                Tasks = _tasks.Select(task => task.CreateMemento()).ToList(),
            };
        }

        internal void Restore(QueueMemento memento, FacilityDefinition facility, LoadContext load)
        {
            NextTaskID = memento.NextTaskID;
            _tasks.Clear();
            _tasks.AddRange(memento.Tasks.Select(task => WorkTask.Restore(this, task, facility, load)));
            Changed();
        }
    }
}
