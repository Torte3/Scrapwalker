using Scrapwalker.Core.Items;

namespace Scrapwalker.Game.UI.Widgets
{
    public readonly struct LootItem
    {
        public LootItem(MaterialDefinition type, Quality grade, int count)
        {
            Type = type;
            Grade = grade;
            Count = count;
        }

        public MaterialDefinition Type { get; }
        public Quality Grade { get; }
        public int Count { get; }
    }
}
