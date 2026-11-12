using Scrapwalker.Core.Items;
using Scrapwalker.Core.Outpost;
using Scrapwalker.Core.Root;

namespace Scrapwalker.Game.UI
{
    public readonly struct BuildCheck
    {
        public BuildCheck(World world, FacilityDefinition definition, QualityFilter filter)
        {
            Allowed = definition.CheckAllowed(world);
            Materials = definition.CheckMaterials(world, filter);
        }

        public CommandResult Allowed { get; }
        public CommandResult Materials { get; }
        public bool IsBuildable => Allowed.IsOK && Materials.IsOK;
    }
}
