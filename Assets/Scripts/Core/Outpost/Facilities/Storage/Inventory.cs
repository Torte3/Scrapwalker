using System;
using System.Collections.Generic;
using Scrapwalker.Core.Framework;
using Scrapwalker.Core.Items;

namespace Scrapwalker.Core.Outpost.Facilities.Storage
{
    public sealed class Inventory
    {
        private readonly StackList _materials = new StackList();
        private readonly Func<int> _capacity;

        internal Inventory(Func<int> capacity)
        {
            _capacity = capacity;
        }

        public int Capacity => _capacity();
        public int FreeSpace => Capacity - MaterialCount;
        public int MaterialCount => _materials.Count;
        public IReadOnlyList<ItemStack> Stacks => _materials.Stacks;

        public int Material(MaterialDefinition type, Quality quality)
        {
            return _materials.CountOf(type, quality);
        }

        public int CountOf(MaterialDefinition type)
        {
            return _materials.CountOf(type);
        }

        internal void Add(ItemStack items)
        {
            _materials.Add(items);
        }

        internal bool TryTakeMaterial(MaterialDefinition type, Quality quality, int count)
        {
            return _materials.TryTake(type, quality, count);
        }

        internal List<ItemStack> TakeAll()
        {
            return _materials.TakeAll();
        }

        internal List<StackMemento> CreateMemento()
        {
            return _materials.CreateMemento();
        }

        internal void Restore(List<StackMemento> memento, LoadContext load)
        {
            _materials.Restore(memento, load);
        }
    }
}
