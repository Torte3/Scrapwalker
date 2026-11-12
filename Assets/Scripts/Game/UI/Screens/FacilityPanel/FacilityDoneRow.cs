using Scrapwalker.Game.UI.Widgets;
using Scrapwalker.Game.Wording.Words;
using UnityEngine.UIElements;

namespace Scrapwalker.Game.UI.Screens.FacilityPanel
{
    internal sealed class FacilityDoneRow
    {
        private readonly Label _title;

        public FacilityDoneRow(TemplateContainer element)
        {
            Element = element;
            _title = element.Need<Label>("recipe-title");
            element.WriteLabel("recipe-time", RoomWords.UpgradeDone);
        }

        public TemplateContainer Element { get; }

        public void Show(string title)
        {
            _title.SetText(title);
        }
    }
}
