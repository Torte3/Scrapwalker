using Scrapwalker.Core.Outpost;
using Scrapwalker.Core.Research;
using Scrapwalker.Game.Definitions;
using Scrapwalker.Game.UI.Widgets;
using UnityEngine;
using UnityEngine.UIElements;

namespace Scrapwalker.Game.UI.Screens.Research
{
    public sealed class ResearchPicture
    {
        private readonly VisualElement _slot;
        private readonly ArtThumb _thumb;

        public ResearchPicture(VisualElement slot, Vector2 size)
        {
            _slot = new VisualElement { pickingMode = PickingMode.Ignore };
            _slot.AddToClassList("research-picture__layer");
            slot.Add(_slot);
            _thumb = new ArtThumb(_slot, size);
        }

        public void Show(GameData data, ResearchDefinition definition)
        {
            ResearchAsset asset = data.Asset(definition);
            if (asset.Picture == null)
            {
                FacilityDefinition facility = asset.Facility(data.Definitions);
                _thumb.Show(facility, data.Asset(facility).Look);
            }
            else
            {
                _thumb.Show(asset.Picture);
            }
        }

        public void SetDim(bool dim)
        {
            _slot.EnableInClassList("art-thumb--dim", dim);
        }
    }
}
