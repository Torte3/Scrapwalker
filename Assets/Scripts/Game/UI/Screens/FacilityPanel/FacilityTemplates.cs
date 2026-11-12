using Scrapwalker.Game.UI.Widgets;
using UnityEngine.UIElements;

namespace Scrapwalker.Game.UI.Screens.FacilityPanel
{
    internal sealed class FacilityTemplates
    {
        public FacilityTemplates(VisualElement root)
        {
            Recipe = root.TakeTemplate("recipe-row-template");
            Done = root.TakeTemplate("done-row-template");
            Task = root.TakeTemplate("order-row-template");
            Material = root.TakeTemplate("material-line-template");
            Stock = root.TakeTemplate("stock-line-template");
        }

        public VisualTreeAsset Recipe { get; }
        public VisualTreeAsset Done { get; }
        public VisualTreeAsset Task { get; }
        public VisualTreeAsset Material { get; }
        public VisualTreeAsset Stock { get; }
    }
}
