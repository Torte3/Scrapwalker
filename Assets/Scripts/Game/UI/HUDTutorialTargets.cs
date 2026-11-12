using Scrapwalker.Core.Outpost;
using Scrapwalker.Core.Outpost.Facilities;
using Scrapwalker.Core.Sessions.Tutorial;
using Scrapwalker.Core.Travel;
using Scrapwalker.Game.UI.Screens.BuildList;
using Scrapwalker.Game.UI.Screens.FacilityPanel;
using Scrapwalker.Game.UI.Screens.Operations;
using Scrapwalker.Game.UI.Screens.Route;
using Scrapwalker.Game.UI.Screens.TopBar;
using Scrapwalker.Game.UI.Screens.Tutorial;
using UnityEngine.UIElements;

namespace Scrapwalker.Game.UI
{
    public sealed class HUDTutorialTargets : ITutorialTargets
    {
        private readonly OperationsScreen _operations;
        private readonly BuildListScreen _buildList;
        private readonly FacilityPanelScreen _facility;
        private readonly RouteScreen _route;
        private readonly TopBarScreen _top;
        private UIContext _context;

        public HUDTutorialTargets(OperationsScreen operations, BuildListScreen buildList, FacilityPanelScreen facility, RouteScreen route, TopBarScreen top)
        {
            _operations = operations;
            _buildList = buildList;
            _facility = facility;
            _route = route;
            _top = top;
        }

        public VisualElement BuildButton => _operations.BuildButton;
        public VisualElement CloseBuildButton => _operations.CloseBuildButton;
        public VisualElement ActionButton => _operations.ActionButton;
        public bool IsFacilityOpen => _facility.IsOpen;
        public VisualElement FacilityCloseButton => _facility.CloseButton;
        public bool IsRouteOpen => _route.IsOpen;

        public void Bind(UIContext context)
        {
            _context = context;
        }

        public VisualElement GuideToCard(FacilityDefinition definition, out TutorialStep step)
        {
            return _buildList.GuideTo(definition, out step);
        }

        public VisualElement GuideToRecipe(RecipeDefinition recipe, int times, out TutorialStep step)
        {
            return _facility.GuideTo(recipe, times, out step);
        }

        public VisualElement FrameOf(RecipeDefinition recipe)
        {
            return _facility.FrameOf(recipe);
        }

        public VisualElement RevealedFor(RecipeDefinition recipe)
        {
            return _facility.RevealedFor(recipe);
        }

        public VisualElement LandButtonOf(RuinDefinition ruin)
        {
            return _route.LandButtonOf(ruin);
        }

        public TutorialTarget AreaOf(TutorialArea area, RecipeDefinition recipe)
        {
            switch (area)
            {
                case TutorialArea.RecipeRow:
                    return new WidgetTarget(_facility.IsShowing(_facility.Work) ? _facility.FrameOf(recipe) : null);
                case TutorialArea.Schedule:
                    return new WidgetTarget(_facility.IsShowing(_facility.Work) ? _facility.Work.Right : null);
                case TutorialArea.ResearchPanel:
                    return new WidgetTarget(_facility.IsShowing(_facility.Research) ? _facility.Frame : null);
                case TutorialArea.BuildMenu:
                    return new WidgetTarget(_buildList.Menu);
                case TutorialArea.Route:
                    return new WidgetTarget(_route.IsOpen ? _route.Frame : null);
                case TutorialArea.WaveStatus:
                    return new WidgetTarget(_top.WaveCell);
                case TutorialArea.Hull:
                    return new WidgetTarget(_top.HullCell);
                case TutorialArea.TaskList:
                    return new WidgetTarget(_facility.IsShowing(_facility.Work) ? _facility.Work.Tasks : null);
                case TutorialArea.Base:
                    return new CellsTarget(_context.World.Grid);
                default:
                    throw new System.ArgumentOutOfRangeException(nameof(area), area, null);
            }
        }
    }
}
