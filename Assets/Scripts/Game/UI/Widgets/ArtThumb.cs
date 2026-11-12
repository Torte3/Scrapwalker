using Scrapwalker.Core.Outpost;
using Scrapwalker.Game.Definitions;
using Scrapwalker.Game.Look.Art;
using UnityEngine;
using UnityEngine.UIElements;

namespace Scrapwalker.Game.UI.Widgets
{
    public sealed class ArtThumb
    {
        private const float CardSlot = 116f;
        private const float Padding = 0.1f;

        private readonly Vector2 _slot;
        private readonly VisualElement _root;
        private readonly ShownValue<object> _shown = new ShownValue<object>();

        public ArtThumb(VisualElement slot) : this(slot, new Vector2(CardSlot, CardSlot))
        {
        }

        public ArtThumb(VisualElement slot, Vector2 size)
        {
            _slot = size;
            _root = slot;
            slot.AddToClassList("art-thumb");
        }

        public void Show(ArtEntry entry)
        {
            if (!_shown.Change(entry))
            {
                return;
            }

            _root.Clear();
            if (entry == null || !TryBoundsOf(entry, out Bounds all))
            {
                return;
            }

            float side = Mathf.Min(_slot.x, _slot.y) * (1f - 2f * Padding);
            float k = side / Mathf.Max(all.size.x, all.size.y);
            foreach (ArtLayer layer in entry.Layers)
            {
                AddLayer(layer.First?.Sprite, all.center, k);
            }
        }

        public void Show(FacilityDefinition definition, FacilityLook look)
        {
            if (!_shown.Change(definition))
            {
                return;
            }

            _root.Clear();
            var size = new Vector2(definition.Width * FacilityArt.CellSize, definition.Height * FacilityArt.CellSize);
            float k = Mathf.Min(1f, Mathf.Min(_slot.x / size.x, _slot.y / size.y));
            AddLayer(look.Idle.FirstFrame?.Sprite, new Vector2(size.x * 0.5f, -size.y * 0.5f), k);
            Place(Element("art-thumb__frame"), (_slot.x - size.x * k) * 0.5f, (_slot.y - size.y * k) * 0.5f, size.x * k, size.y * k);
        }

        private static bool TryBoundsOf(ArtEntry entry, out Bounds all)
        {
            all = default;
            bool any = false;
            foreach (ArtLayer layer in entry.Layers)
            {
                Sprite sprite = layer.First?.Sprite;
                if (sprite == null)
                {
                    continue;
                }

                if (any)
                {
                    all.Encapsulate(sprite.bounds);
                }
                else
                {
                    all = sprite.bounds;
                    any = true;
                }
            }

            return any;
        }

        private void AddLayer(Sprite sprite, Vector2 center, float k)
        {
            if (sprite == null)
            {
                return;
            }

            Bounds b = sprite.bounds;
            VisualElement e = Element("art-thumb__layer");
            e.style.backgroundImage = new StyleBackground(sprite);
            Place(e, _slot.x * 0.5f + (b.min.x - center.x) * k, _slot.y * 0.5f - (b.max.y - center.y) * k, b.size.x * k, b.size.y * k);
        }

        private VisualElement Element(string className)
        {
            var e = new VisualElement { pickingMode = PickingMode.Ignore };
            e.AddToClassList(className);
            _root.Add(e);
            return e;
        }

        private void Place(VisualElement e, float x, float y, float width, float height)
        {
            e.style.left = Length.Percent(x / _slot.x * 100f);
            e.style.top = Length.Percent(y / _slot.y * 100f);
            e.style.width = Length.Percent(width / _slot.x * 100f);
            e.style.height = Length.Percent(height / _slot.y * 100f);
        }
    }
}
