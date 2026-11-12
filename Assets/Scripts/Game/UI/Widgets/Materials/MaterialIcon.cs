using Scrapwalker.Game.Look.Art.Sets;
using UnityEngine;
using UnityEngine.UIElements;

namespace Scrapwalker.Game.UI.Widgets.Materials
{
    public static class MaterialIcon
    {
        public static void SetEnergy(this VisualElement element, ScreenArt art)
        {
            Put(element, art.Energy);
        }

        private static void Put(VisualElement element, Sprite sprite)
        {
            element.style.backgroundImage = sprite != null ? new StyleBackground(sprite) : new StyleBackground(StyleKeyword.None);
        }
    }
}
