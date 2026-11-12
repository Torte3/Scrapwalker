using System.Collections.Generic;
using Scrapwalker.Core.Items;
using Scrapwalker.Core.Outpost.Facilities.Recipes;

namespace Scrapwalker.Core.Outpost.Facilities
{
    internal sealed class InputAllocation
    {
        private static readonly StackList NoStacks = new StackList();

        private readonly List<InputShare> _shares = new List<InputShare>();
        private readonly Dictionary<WorkTask, StackList> _stacks = new Dictionary<WorkTask, StackList>();
        private readonly Dictionary<WorkTask, int> _missing = new Dictionary<WorkTask, int>();

        public IReadOnlyList<InputShare> Shares => _shares;

        public StackList StacksFor(WorkTask task)
        {
            return _stacks.TryGetValue(task, out StackList stacks) ? stacks : NoStacks;
        }

        public int Missing(WorkTask task)
        {
            return _missing.TryGetValue(task, out int missing) ? missing : 0;
        }

        internal void Add(WorkTask task, MaterialAmount need, StackList taken)
        {
            _shares.Add(new InputShare(task, need, taken.Count));
            _missing.TryGetValue(task, out int missing);
            _missing[task] = missing + need.Count - taken.Count;
            if (!_stacks.TryGetValue(task, out StackList stacks))
            {
                stacks = new StackList();
                _stacks.Add(task, stacks);
            }

            stacks.AddRange(taken.Stacks);
        }
    }
}
