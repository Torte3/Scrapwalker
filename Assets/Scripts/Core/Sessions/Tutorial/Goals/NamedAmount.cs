using Scrapwalker.Core.Items;
using Scrapwalker.Core.Root;

namespace Scrapwalker.Core.Sessions.Tutorial.Goals
{
    public readonly struct NamedAmount
    {
        public NamedAmount(string material, int count)
        {
            Material = new NamedDefinition<MaterialDefinition>(material);
            Count = count;
        }

        public NamedDefinition<MaterialDefinition> Material { get; }
        public int Count { get; }

        public MaterialDefinition Of(World world)
        {
            return Material.Of(world);
        }
    }
}
