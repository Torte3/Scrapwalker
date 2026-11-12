using Scrapwalker.Core.Research;

namespace Scrapwalker.Core.Outpost.Facilities.Requirements
{
    public interface IRequirementSupply
    {
        bool IsDone(ResearchDefinition research);

        bool CanRun(RecipeDefinition recipe);
    }
}
