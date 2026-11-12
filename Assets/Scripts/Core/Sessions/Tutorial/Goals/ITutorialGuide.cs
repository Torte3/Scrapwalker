using Scrapwalker.Core.Items;
using Scrapwalker.Core.Outpost;
using Scrapwalker.Core.Outpost.Facilities;
using Scrapwalker.Core.Travel;

namespace Scrapwalker.Core.Sessions.Tutorial.Goals
{
    public interface ITutorialGuide<T>
    {
        T Nothing();
        T Place(FacilityDefinition facility);
        T Order(FacilityDefinition facility, RecipeDefinition next, int times, MaterialDefinition making);
        T ChooseRuin(RuinDefinition ruin);
    }
}
