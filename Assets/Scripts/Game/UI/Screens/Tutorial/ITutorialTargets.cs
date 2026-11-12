using Scrapwalker.Core.Outpost;
using Scrapwalker.Core.Outpost.Facilities;
using Scrapwalker.Core.Sessions.Tutorial;
using Scrapwalker.Core.Travel;
using UnityEngine.UIElements;

namespace Scrapwalker.Game.UI.Screens.Tutorial
{
    public interface ITutorialTargets
    {
        VisualElement BuildButton { get; }
        VisualElement CloseBuildButton { get; }
        VisualElement ActionButton { get; }

        VisualElement GuideToCard(FacilityDefinition definition, out TutorialStep step);

        bool IsFacilityOpen { get; }
        VisualElement GuideToRecipe(RecipeDefinition recipe, int times, out TutorialStep step);
        VisualElement FrameOf(RecipeDefinition recipe);
        VisualElement RevealedFor(RecipeDefinition recipe);
        VisualElement FacilityCloseButton { get; }

        bool IsRouteOpen { get; }
        VisualElement LandButtonOf(RuinDefinition ruin);

        TutorialTarget AreaOf(TutorialArea area, RecipeDefinition recipe);
    }
}
