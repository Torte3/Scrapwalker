using System.Collections.Generic;
using Scrapwalker.Game.UI.Widgets;
using UnityEngine.UIElements;

namespace Scrapwalker.Game.UI.Screens.Ticker
{
    public sealed class TickerScreen : UIScreen
    {
        private const float SpeedPixelsPerSecond = 200f;
        private const float Gap = 64f;

        private readonly VisualElement _band;
        private readonly VisualTreeAsset _itemTemplate;
        private readonly List<TickerItem> _items = new List<TickerItem>();

        public TickerScreen(VisualElement root) : base(root)
        {
            _band = Q("ticker");
            _itemTemplate = Root.TakeTemplate("ticker-item-template");
        }

        public void Clear()
        {
            foreach (TickerItem item in _items)
            {
                item.Root.RemoveFromHierarchy();
            }

            _items.Clear();
            _band.Display(false);
        }

        protected override void OnBind()
        {
            Clear();
        }

        public void Push(string text, IReadOnlyList<LootItem> items)
        {
            // TemplateContainer ごと入れると絶対位置にならない
            VisualElement row = _itemTemplate.Instantiate()[0];
            Q<Label>(row, "text").text = text;
            new LootView(Q<VisualElement>(row, "items")).Show(Data, items);
            _band.Add(row);
            _items.Add(new TickerItem(row));
            _band.Display(true);
        }

        public void Refresh()
        {
            if (_items.Count == 0)
            {
                return;
            }

            float bandWidth = _band.resolvedStyle.width;
            if (!float.IsNaN(bandWidth))
            {
                PlaceNew(bandWidth);
            }

            float distance = SpeedPixelsPerSecond * App.GameDelta;
            for (int i = _items.Count - 1; i >= 0; i--)
            {
                if (_items[i].IsPlaced && !_items[i].Scroll(distance))
                {
                    _items[i].Root.RemoveFromHierarchy();
                    _items.RemoveAt(i);
                }
            }

            if (_items.Count == 0)
            {
                _band.Display(false);
            }
        }

        private void PlaceNew(float bandWidth)
        {
            for (int i = 0; i < _items.Count; i++)
            {
                if (_items[i].IsPlaced)
                {
                    continue;
                }

                if (!_items[i].TryPlace(bandWidth, i > 0 ? _items[i - 1] : null, Gap))
                {
                    return;
                }
            }
        }
    }
}
