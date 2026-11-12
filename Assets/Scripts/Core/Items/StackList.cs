using System.Collections.Generic;
using System.Linq;
using Scrapwalker.Core.Framework;

namespace Scrapwalker.Core.Items
{
    public sealed class StackList
    {
        private readonly List<ItemStack> _stacks = new List<ItemStack>();

        public StackList()
        {
        }

        public StackList(IEnumerable<ItemStack> stacks)
        {
            AddRange(stacks);
        }

        public IReadOnlyList<ItemStack> Stacks => _stacks;
        public int Count => _stacks.Sum(stack => stack.Count);
        public bool IsEmpty => _stacks.Count == 0;
        internal int Version { get; private set; }

        public static StackList Of(IReadOnlyList<MaterialAmount> cost, Quality quality)
        {
            return new StackList(cost.Select(amount => ItemStack.Of(amount.Type, quality, amount.Count)));
        }

        internal List<StackMemento> CreateMemento()
        {
            return _stacks.Select(stack => stack.CreateMemento()).ToList();
        }

        internal void Restore(List<StackMemento> memento, LoadContext load)
        {
            _stacks.Clear();
            AddRange(memento.Select(stack => ItemStack.Restore(stack, load)));
        }

        public int CountOf(MaterialDefinition type)
        {
            return _stacks.Where(stack => stack.Type == type).Sum(stack => stack.Count);
        }

        public int CountOf(MaterialDefinition type, Quality quality)
        {
            int index = IndexOf(type, quality);
            return index >= 0 ? _stacks[index].Count : 0;
        }

        public bool Covers(IReadOnlyList<MaterialAmount> cost)
        {
            return MaterialAmount.PerMaterial(cost).All(amount => CountOf(amount.Type) >= amount.Count);
        }

        public Quality? AverageQuality => _stacks.Count > 0 ? _stacks.Average() : (Quality?)null;

        internal void Add(ItemStack items)
        {
            if (items.IsEmpty)
            {
                return;
            }

            int index = IndexOf(items.Type, items.Quality);
            if (index >= 0)
            {
                _stacks[index] = items.WithCount(_stacks[index].Count + items.Count);
            }
            else
            {
                _stacks.Add(items);
            }

            Version++;
        }

        internal void AddRange(IEnumerable<ItemStack> stacks)
        {
            foreach (ItemStack stack in stacks)
            {
                Add(stack);
            }
        }

        internal bool TryTake(MaterialDefinition type, Quality quality, int count)
        {
            if (count <= 0)
            {
                return true;
            }

            int index = IndexOf(type, quality);
            if (index < 0 || _stacks[index].Count < count)
            {
                return false;
            }

            if (_stacks[index].Count == count)
            {
                _stacks.RemoveAt(index);
            }
            else
            {
                _stacks[index] = _stacks[index].WithCount(_stacks[index].Count - count);
            }

            Version++;
            return true;
        }

        internal List<ItemStack> RemoveAll(System.Predicate<ItemStack> match)
        {
            List<ItemStack> removed = _stacks.FindAll(match);
            _stacks.RemoveAll(match);
            Version++;
            return removed;
        }

        internal List<ItemStack> TakeAll()
        {
            var taken = new List<ItemStack>(_stacks);
            Clear();
            return taken;
        }

        internal void Clear()
        {
            _stacks.Clear();
            Version++;
        }

        private int IndexOf(MaterialDefinition type, Quality quality)
        {
            return _stacks.FindIndex(stack => stack.Type == type && stack.Quality == quality);
        }
    }
}
