using Scrapwalker.Core.Framework;

namespace Scrapwalker.Core.Items
{
    public readonly struct ItemStack
    {
        public MaterialDefinition Type { get; }
        public Quality Quality { get; }
        public int Count { get; }

        public bool IsEmpty => Count <= 0;

        internal string LogText => IsEmpty ? "なし" : Type + "×" + Count + "(" + Quality + ")";

        private ItemStack(MaterialDefinition type, Quality quality, int count)
        {
            Type = type;
            Quality = quality;
            Count = count;
        }

        public static ItemStack Of(MaterialDefinition type, Quality quality, int count)
        {
            return count > 0 ? new ItemStack(type, quality, count) : default;
        }

        internal StackMemento CreateMemento()
        {
            return new StackMemento { Type = Type?.ID, Quality = Quality, Count = Count };
        }

        internal static ItemStack Restore(StackMemento memento, LoadContext load)
        {
            return memento.Count > 0 ? Of(load.Find<MaterialDefinition>(memento.Type), memento.Quality, memento.Count) : default;
        }

        public bool IsSameItemAs(ItemStack other)
        {
            return !IsEmpty && !other.IsEmpty && Type == other.Type && Quality == other.Quality;
        }

        public ItemStack WithCount(int count)
        {
            return !IsEmpty ? Of(Type, Quality, count) : default;
        }
    }
}
