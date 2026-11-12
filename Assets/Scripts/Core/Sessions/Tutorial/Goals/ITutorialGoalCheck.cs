using Scrapwalker.Core.Outpost;

namespace Scrapwalker.Core.Sessions.Tutorial.Goals
{
    public interface ITutorialGoalCheck
    {
        void MissingFacility(string name);
        void MissingMaterial(string name);
        void MissingRuin(string name);
        void MissingRecipe(FacilityDefinition facility, string what);
    }
}
