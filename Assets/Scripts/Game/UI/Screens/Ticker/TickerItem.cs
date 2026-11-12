using UnityEngine;
using UnityEngine.UIElements;

namespace Scrapwalker.Game.UI.Screens.Ticker
{
    internal sealed class TickerItem
    {
        private float _x;
        private float _width;

        public TickerItem(VisualElement root)
        {
            Root = root;
        }

        public VisualElement Root { get; }
        public bool IsPlaced { get; private set; }
        private float RightEnd => _x + _width;

        public bool TryPlace(float bandWidth, TickerItem previous, float gap)
        {
            float width = Root.resolvedStyle.width;
            if (float.IsNaN(width) || width <= 0f)
            {
                return false;
            }

            _width = width;
            _x = previous != null ? Mathf.Max(bandWidth, previous.RightEnd + gap) : bandWidth;
            IsPlaced = true;
            Root.RemoveFromClassList("invisible");
            return true;
        }

        public bool Scroll(float distance)
        {
            _x -= distance;
            if (RightEnd < 0f)
            {
                return false;
            }

            Root.style.translate = new Translate(_x, 0f);
            return true;
        }
    }
}
