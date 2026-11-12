using Scrapwalker.Core.Items;
using Scrapwalker.Core.Outpost;
using Scrapwalker.Core.Outpost.Facilities;
using Scrapwalker.Core.Root;
using Scrapwalker.Core.Sessions;
using Scrapwalker.Core.Sessions.Tutorial;
using Scrapwalker.Core.Sessions.Tutorial.Goals;
using Scrapwalker.Core.Travel;
using Scrapwalker.Game.UI;
using Scrapwalker.Game.UI.Screens.Tutorial;
using UnityEngine.UIElements;
using MaterialDefinition = Scrapwalker.Core.Items.MaterialDefinition;

namespace Scrapwalker.Game.App
{
    public sealed class TutorialGuide : ITutorialGuide<TutorialCue>
    {
        private readonly Session _session;
        private readonly UIState _ui;
        private readonly ITutorialTargets _hud;

        public TutorialGuide(Session session, UIState ui, ITutorialTargets hud)
        {
            _session = session;
            _ui = ui;
            _hud = hud;
        }

        private World World => _session.World;

        public TutorialCue Cue()
        {
            return _session.Tutorial.IsWaiting ? _session.Tutorial.Progress.CurrentGoal.Guide(World, this) : default;
        }

        TutorialCue ITutorialGuide<TutorialCue>.Nothing()
        {
            return TryExplain(TutorialStep.None, null, null, out TutorialCue explanation) ? explanation : default;
        }

        TutorialCue ITutorialGuide<TutorialCue>.Place(FacilityDefinition definition)
        {
            if (_session.Tutorial.Progress.IsCurrentDone)
            {
                return _ui.IsBuildMode ? Widget(TutorialStep.CloseBuild, _hud.CloseBuildButton) : default;
            }

            if (!_ui.IsBuildMode)
            {
                return Widget(TutorialStep.OpenBuild, _hud.BuildButton);
            }

            if (_ui.Build.PlacingDefinition == definition)
            {
                return new TutorialCue(TutorialStep.PlaceOnBase, new CellsTarget(World.Grid));
            }

            VisualElement card = _hud.GuideToCard(definition, out TutorialStep step);
            return Point(step, card, null, null);
        }

        TutorialCue ITutorialGuide<TutorialCue>.Order(FacilityDefinition definition, RecipeDefinition next, int times, MaterialDefinition making)
        {
            Facility facility = World.Facilities.FirstOperational(definition);
            if (_session.Tutorial.Progress.IsCurrentDone)
            {
                return _hud.IsFacilityOpen ? Widget(TutorialStep.ClosePanel, _hud.FacilityCloseButton).Revealing(_hud.RevealedFor(next)) : default;
            }

            if (facility == null)
            {
                return default;
            }

            if (!_hud.IsFacilityOpen || _ui.Selected != facility)
            {
                return FacilityCue(facility);
            }

            VisualElement entry = _hud.GuideToRecipe(next, times, out TutorialStep step);
            if (TryExplain(step, making, next, out TutorialCue explanation))
            {
                return explanation;
            }

            return Widget(step, entry).Within(_hud.FrameOf(next)).Revealing(_hud.RevealedFor(next)).Making(making);
        }

        TutorialCue ITutorialGuide<TutorialCue>.ChooseRuin(RuinDefinition ruin)
        {
            if (ruin == null || _session.Tutorial.Progress.IsCurrentDone)
            {
                return default;
            }

            if (!_hud.IsRouteOpen)
            {
                return Widget(TutorialStep.OpenRoute, _hud.ActionButton);
            }

            return Point(TutorialStep.ChooseRuin, _hud.LandButtonOf(ruin), null, null);
        }

        private TutorialCue Point(TutorialStep step, VisualElement widget, MaterialDefinition making, RecipeDefinition recipe)
        {
            return TryExplain(step, making, recipe, out TutorialCue explanation) ? explanation : Widget(step, widget);
        }

        private bool TryExplain(TutorialStep step, MaterialDefinition making, RecipeDefinition recipe, out TutorialCue cue)
        {
            cue = default;
            if (!_session.Tutorial.TryGetExplanation(step, making, out TutorialExplanation explanation))
            {
                return false;
            }

            TutorialTarget area = _hud.AreaOf(explanation.Area, recipe);
            if (area == null || !area.IsShown)
            {
                return false;
            }

            cue = TutorialCue.Explaining(area, explanation.Text);
            return true;
        }


        private static TutorialCue FacilityCue(Facility facility)
        {
            return new TutorialCue(TutorialStep.OpenFacility, new CellsTarget(facility.Rect));
        }

        private static TutorialCue Widget(TutorialStep step, VisualElement widget)
        {
            return new TutorialCue(step, new WidgetTarget(widget));
        }
    }
}
