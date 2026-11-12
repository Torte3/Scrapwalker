using System.Collections.Generic;
using Scrapwalker.Core.Framework;
using Scrapwalker.Core.Items;
using Scrapwalker.Core.Outpost.Bots;
using Scrapwalker.Core.Outpost.Facilities.Recipes;

namespace Scrapwalker.Core.Outpost.Facilities
{
    public sealed class Workshop
    {
        private InputAllocation _allocation;
        private (int Queue, int Inputs, WorkJob Job) _allocatedFor;

        public Workshop(FacilityDefinition definition)
        {
            Upgrades = new InstalledUpgrades(definition);
        }

        public WorkQueue Queue { get; } = new WorkQueue();
        public StackList Inputs { get; } = new StackList();
        public WorkJob Job { get; internal set; }
        public InstalledUpgrades Upgrades { get; }

        internal WorkshopMemento CreateMemento()
        {
            return new WorkshopMemento
            {
                Queue = Queue.CreateMemento(),
                Inputs = Inputs.CreateMemento(),
                WorkingTask = Job?.Task.ID,
                Job = Job?.CreateMemento(),
                Upgrades = Upgrades.CreateMemento(),
            };
        }

        internal void Restore(WorkshopMemento memento, FacilityDefinition facility, LoadContext load)
        {
            Queue.Restore(memento.Queue, facility, load);
            Inputs.Restore(memento.Inputs, load);
            Job = null;
            if (memento.Job != null)
            {
                WorkTask working = memento.WorkingTask.HasValue ? Queue.Find(memento.WorkingTask.Value) : null;
                Job = WorkJob.Restore(memento.Job, working ?? throw load.Malformed(), load);
            }

            Upgrades.Restore(memento.Upgrades, load);
        }

        public WorkTask WorkingTask => Job?.Task;
        public float WorkProgress => Job?.Progress ?? 0f;
        public WorkTask CurrentTask => WorkingTask ?? Queue.First;
        public WorkPriority CurrentTaskPriority => CurrentTask != null ? CurrentTask.Recipe.Priority : WorkPriority.Production;
        public bool IsUpgrading => WorkingTask?.Recipe.Result.WorksOnRoom == true;

        public float ProgressOf(WorkTask task)
        {
            return task == WorkingTask ? WorkProgress : task.HeldJob?.Progress ?? 0f;
        }

        public bool HasStarted(WorkTask task)
        {
            return task == WorkingTask || task.HeldJob != null;
        }

        internal bool ShouldHold(bool inCombat)
        {
            return Job != null && !Job.Task.Recipe.CanRunNow(inCombat) && Queue.First != Job.Task;
        }

        internal void Hold()
        {
            Job.Task.HeldJob = Job;
            Job = null;
        }

        internal void Resume(WorkTask task)
        {
            Job = task.HeldJob;
            task.HeldJob = null;
        }

        public int FedCount(WorkTask task, MaterialDefinition type)
        {
            if (HasStarted(task))
            {
                return MaterialAmount.CountIn(task.Recipe.Inputs, type);
            }

            return Allocate().StacksFor(task).CountOf(type);
        }

        public bool InputsReady(WorkTask task, out int have, out int need)
        {
            need = MaterialAmount.Total(task.Recipe.DeliveredInputs);
            if (HasStarted(task))
            {
                have = need;
                return true;
            }

            InputAllocation allocation = Allocate();
            have = allocation.StacksFor(task).Count;
            return IsFed(task, allocation);
        }

        public float ReadyShare(WorkTask task)
        {
            InputsReady(task, out int have, out int need);
            return need == 0 ? 1f : (float)have / need;
        }

        internal bool HasInputsFor(WorkTask task)
        {
            return IsFed(task, Allocate());
        }

        internal bool TryBeginWithInputs(WorkTask task, int ticks)
        {
            InputAllocation allocation = Allocate();
            if (!IsFed(task, allocation))
            {
                return false;
            }

            StackList stacks = allocation.StacksFor(task);
            foreach (ItemStack stack in stacks.Stacks)
            {
                Inputs.TryTake(stack.Type, stack.Quality, stack.Count);
            }

            Begin(task, stacks, ticks);
            return true;
        }

        internal void Begin(WorkTask task, StackList stacks, int ticks)
        {
            Job = WorkJob.Start(task, stacks, ticks);
        }

        private bool IsFed(WorkTask task, InputAllocation allocation)
        {
            return Queue.IndexOf(task) >= 0 && !HasStarted(task) && allocation.Missing(task) == 0;
        }

        internal InputAllocation Allocate()
        {
            (int, int, WorkJob) state = (Queue.Version, Inputs.Version, Job);
            if (_allocation == null || !_allocatedFor.Equals(state))
            {
                _allocation = BuildAllocation();
                _allocatedFor = state;
            }

            return _allocation;
        }

        private InputAllocation BuildAllocation()
        {
            var allocation = new InputAllocation();
            var spare = new StackList(Inputs.Stacks);
            foreach (WorkTask task in Queue.Tasks)
            {
                if (HasStarted(task))
                {
                    continue;
                }

                foreach (MaterialAmount amount in task.Recipe.DeliveredInputs)
                {
                    allocation.Add(task, amount, TakeMatching(spare, amount.Type, task.Filter, amount.Count));
                }
            }

            return allocation;
        }

        private static StackList TakeMatching(StackList spare, MaterialDefinition type, QualityFilter filter, int want)
        {
            var taken = new StackList();
            while (taken.Count < want)
            {
                ItemStack? best = null;
                foreach (ItemStack stack in spare.Stacks)
                {
                    if (stack.Type == type && filter.Accepts(stack.Quality) && (!best.HasValue || filter.Prefers(stack.Quality, best.Value.Quality)))
                    {
                        best = stack;
                    }
                }

                if (!best.HasValue)
                {
                    break;
                }

                int count = System.Math.Min(best.Value.Count, want - taken.Count);
                spare.TryTake(type, best.Value.Quality, count);
                taken.Add(best.Value.WithCount(count));
            }

            return taken;
        }
    }
}
