using Scrapwalker.Core.Outpost;
using Scrapwalker.Game.Definitions;
using Scrapwalker.Game.UI.Widgets.Materials;
using Scrapwalker.Game.Wording;
using Scrapwalker.Game.UI.Widgets;
using UnityEngine.UIElements;

namespace Scrapwalker.Game.UI.Screens.BuildList
{
    public sealed class BuildCard
    {
        public const int LineCount = 4;

        private readonly VisualElement _thumbSlot;
        private readonly ArtThumb _thumb;
        private readonly Label _name;
        private readonly Label _size;
        private readonly VisualElement _ring;

        public BuildCard(VisualElement instance)
        {
            Button = instance.Need<Button>("card");
            _thumbSlot = instance.Need<VisualElement>("thumb");
            _thumb = new ArtThumb(_thumbSlot);
            _name = instance.Need<Label>("name");
            _size = instance.Need<Label>("size");
            _ring = instance.Need<VisualElement>("ring");
            VisualElement lines = instance.Need<VisualElement>("lines");
            Lines = new MaterialList(lines, lines.TakeTemplate("line-template"), "build-card__line");
        }

        public Button Button { get; }

        public MaterialList Lines { get; }

        public void SetVisible(bool isVisible)
        {
            Button.Show(isVisible);
        }

        public void Set(GameData data, FacilityDefinition definition, bool isBuildable, bool isChosen)
        {
            _name.SetText(data.NameOf(definition));
            _size.SetText(definition.Size());
            Button.EnableInClassList("build-card--blocked", !isBuildable);
            Button.EnableInClassList("build-card--chosen", isChosen);
            _ring.Display(isChosen);
            _thumb.Show(definition, data.Asset(definition).Look);
            _thumbSlot.EnableInClassList("art-thumb--dim", !isBuildable);
        }
    }
}
