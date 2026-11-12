using System.Collections.Generic;
using System.Linq;
using Scrapwalker.Core.Outpost;
using Scrapwalker.Core.Outpost.Facilities;
using Scrapwalker.Core.Sessions.Tutorial;
using Scrapwalker.Game.UI.Screens.Research;
using Scrapwalker.Game.UI.Screens.Tutorial;
using Scrapwalker.Game.UI.Widgets;
using Scrapwalker.Game.Wording.Words;
using UnityEngine.UIElements;

namespace Scrapwalker.Game.UI.Screens.FacilityPanel
{
    public sealed class FacilityPanelScreen : UIScreen
    {
        private readonly WindowFrame _frame;
        private readonly VisualElement _panel;
        private readonly VisualElement _tabs;
        private readonly FacilityPage[] _all;
        private readonly ButtonChoice<FacilityPage> _choice;
        private readonly List<FacilityPage> _open = new List<FacilityPage>();
        private FacilityPage _active;
        private int _shownID = -1;

        public FacilityPanelScreen(VisualElement root) : base(root)
        {
            _panel = Q("room-panel");
            _frame = new WindowFrame(_panel);
            _frame.HeadExtra.Add(Q("room-head"));
            _tabs = Q("tabs");
            root.WriteLabel("tab-research-label", RoomWords.TabResearch);
            root.WriteLabel("tab-work-label", RoomWords.TabWork);
            var templates = new FacilityTemplates(root);
            Research = AddPart(new ResearchPage(Q("research-host")));
            Work = AddPart(new FacilityWorkPage(Q("work-page"), templates));
            VisualElement pageHeads = Q("page-heads");
            _all = new FacilityPage[] { Research, Work };
            _choice = new ButtonChoice<FacilityPage>("tab--selected", Choose);
            AddTab(Research, Q<Button>("tab-research"), pageHeads);
            AddTab(Work, Q<Button>("tab-work"), pageHeads);
        }

        public bool IsOpen { get; private set; }
        public Button CloseButton => _frame.CloseButton;
        public VisualElement Frame => _frame.Frame;
        public ResearchPage Research { get; }
        public FacilityWorkPage Work { get; }

        public bool HasPage(FacilityPage page)
        {
            return IsOpen && _open.Contains(page);
        }

        public bool IsShowing(FacilityPage page)
        {
            return IsOpen && _active == page;
        }

        public VisualElement GuideTo(RecipeDefinition recipe, int times, out TutorialStep step)
        {
            FacilityPage page = PageOf(recipe);
            if (IsShowing(page))
            {
                return page.GuideTo(recipe, times, out step);
            }

            step = TutorialStep.ChooseTab;
            return _choice.ButtonOf(page);
        }

        public VisualElement FrameOf(RecipeDefinition recipe)
        {
            FacilityPage page = PageOf(recipe);
            return IsShowing(page) ? page.FrameOf(recipe) : null;
        }

        public VisualElement RevealedFor(RecipeDefinition recipe)
        {
            FacilityPage page = recipe != null ? PageOf(recipe) : Work;
            return IsShowing(page) ? page.Revealed : null;
        }

        private FacilityPage PageOf(RecipeDefinition recipe)
        {
            return _all.First(page => page.Lists(recipe));
        }

        protected override void OnBind()
        {
            _shownID = -1;
            if (IsOpen)
            {
                Close();
            }
        }

        public void Refresh(Facility f)
        {
            if (f == null)
            {
                if (IsOpen)
                {
                    Close();
                }

                return;
            }

            if (!IsOpen || f.ID != _shownID)
            {
                Open(f);
            }

            _active.Refresh(f);
        }

        private void AddTab(FacilityPage page, Button button, VisualElement pageHeads)
        {
            if (page.Head != null)
            {
                pageHeads.Add(page.Head);
            }

            _choice.Add(page, button, on => ShowPage(page, on));
        }

        private void ShowPage(FacilityPage page, bool on)
        {
            if (page.PanelClass != null)
            {
                _panel.EnableInClassList(page.PanelClass, on);
            }

            page.Root.Display(on);
            page.Head?.Display(on);
        }

        private void Open(Facility f)
        {
            _shownID = f.ID;
            _open.Clear();
            if (f.Definition.Recipes.Exists(Research.Lists))
            {
                _open.Add(Research);
            }

            _open.Add(Work);
            bool hasTabs = _open.Count > 1;
            foreach (FacilityPage page in _all)
            {
                _choice.ButtonOf(page).Display(hasTabs && _open.Contains(page));
            }

            _tabs.Display(hasTabs);
            foreach (FacilityPage page in _open)
            {
                page.Open(f);
            }

            if (IsOpen)
            {
                _frame.SetTitle(Data.NameOf(f.Definition));
            }
            else
            {
                IsOpen = true;
                _frame.Open(Data.NameOf(f.Definition));
                _frame.SetCloseAction(() => UIState.Deselect());
            }

            Choose(_open[0]);
        }

        private void Choose(FacilityPage chosen)
        {
            if (!_open.Contains(chosen))
            {
                return;
            }

            _active = chosen;
            _choice.Select(chosen);
            _frame.ShowHeadExtra(_open.Count > 1 || chosen.Head != null);
        }

        private void Close()
        {
            IsOpen = false;
            _shownID = -1;
            _frame.Close();
            foreach (FacilityPage page in _open)
            {
                page.Close();
            }

            _open.Clear();
            _active = null;
        }
    }
}
