using Scrapwalker.Core.Outpost;
using Scrapwalker.Core.Outpost.Facilities;
using Scrapwalker.Core.Sessions.Tutorial;
using UnityEngine.UIElements;

namespace Scrapwalker.Game.UI.Screens.FacilityPanel
{
    public abstract class FacilityPage : UIScreen
    {
        protected FacilityPage(VisualElement root) : base(root)
        {
        }

        public virtual VisualElement Head => null;

        public virtual string PanelClass => null;

        public abstract VisualElement Revealed { get; }

        public abstract void Open(Facility f);

        public abstract void Close();

        public abstract void Refresh(Facility f);

        public abstract bool Lists(RecipeDefinition recipe);

        public abstract VisualElement GuideTo(RecipeDefinition recipe, int times, out TutorialStep step);

        public abstract VisualElement FrameOf(RecipeDefinition recipe);
    }
}
