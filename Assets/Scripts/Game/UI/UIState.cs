using Scrapwalker.Core.Outpost;
using Scrapwalker.Core.Outpost.Grid;
using Scrapwalker.Core.Travel;
using Scrapwalker.Game.App;
using Scrapwalker.Game.UI.Build;

namespace Scrapwalker.Game.UI
{
    public sealed class UIState
    {
        private bool _atRelocateTarget;

        public UIState(BuildTool build)
        {
            Build = build;
        }

        public BuildTool Build { get; }
        public Facility Selected { get; private set; }
        public bool IsSelectedAtRelocateTarget => _atRelocateTarget && Selected != null && Selected.Condition.RelocateTarget.HasValue;
        public bool IsBuildMode { get; private set; }
        public bool IsRouteWanted { get; private set; }
        public bool IsSideOpen { get; private set; }
        public SidePage SidePage { get; private set; }

        public Facility PanelFacility => ShownSelection != null && !IsBuildMode && ShownSelection.IsOperational && HasPanelContent(ShownSelection) ? ShownSelection : null;

        public Facility MenuFacility => PanelFacility == null ? ShownSelection : null;

        public bool StopsTime => IsBuildMode || PanelFacility != null || IsRouteShown(null);

        public bool IsIdle => !IsBuildMode && !Build.IsActive && Selected == null && !IsSideOpen && !IsRouteShown(null);

        private Facility ShownSelection => Build.IsActive ? null : Selected;

        private static bool HasPanelContent(Facility f)
        {
            return f.Definition.HasRecipes || f.Inventory != null || !f.Definition.IsIndestructible;
        }

        public bool IsRouteShown(RuinDefinition spoken)
        {
            return (IsRouteWanted || spoken != null) && PanelFacility == null && !IsBuildMode;
        }

        public void Reset()
        {
            SetSelected(null, false);
            IsBuildMode = false;
            Build.Cancel();
            IsRouteWanted = false;
            IsSideOpen = false;
        }

        public void Select(Facility f)
        {
            SetSelected(f, false);
        }

        public void SelectAtRelocateTarget(Facility f)
        {
            SetSelected(f, true);
        }

        public void Deselect()
        {
            SetSelected(null, false);
            if (Build.Gesture.EndsOnDeselect)
            {
                Build.Cancel();
            }
        }

        public void RunBuild(Cell cursor, bool keepPlacing)
        {
            bool deselects = Build.Gesture.DeselectsAfterRun;
            if (Build.TryRun(cursor, keepPlacing) && deselects)
            {
                Select(null);
            }
        }

        public void Listen(GameEventListeners listeners)
        {
            listeners.On<FacilityRemoved>(removed => OnFacilityRemoved(removed.FacilityID));
            listeners.On<TookOff>(_ => Reset());
        }

        private void OnFacilityRemoved(int facilityID)
        {
            if (Selected != null && Selected.ID == facilityID)
            {
                SetSelected(null, false);
            }
        }

        public void SetBuildMode(bool on)
        {
            if (IsBuildMode == on)
            {
                return;
            }

            IsBuildMode = on;
            Build.Cancel();
            SetSelected(null, false);
        }

        public void BeginPlace(FacilityDefinition definition)
        {
            IsBuildMode = true;
            SetSelected(null, false);
            Build.BeginPlace(definition);
        }

        public void ToggleRoute()
        {
            IsRouteWanted = !IsRouteWanted;
        }

        public void CloseRoute()
        {
            IsRouteWanted = false;
        }

        public void OpenSide(SidePage page)
        {
            if (IsSideOpen && SidePage == page)
            {
                IsSideOpen = false;
                return;
            }

            IsSideOpen = true;
            SidePage = page;
        }

        public void Back()
        {
            if (Build.IsActive)
            {
                Build.Cancel();
            }
            else if (IsBuildMode && Selected != null)
            {
                Select(null);
            }
            else if (IsRouteWanted)
            {
                IsRouteWanted = false;
            }
            else if (IsSideOpen)
            {
                IsSideOpen = false;
            }
            else if (IsBuildMode)
            {
                SetBuildMode(false);
            }
            else
            {
                Select(null);
            }
        }

        private void SetSelected(Facility f, bool atRelocateTarget)
        {
            Selected = f;
            _atRelocateTarget = atRelocateTarget;
        }
    }
}
