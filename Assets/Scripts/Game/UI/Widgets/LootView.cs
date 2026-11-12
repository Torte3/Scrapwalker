using System.Collections.Generic;
using Scrapwalker.Game.Definitions;
using Scrapwalker.Game.Wording;
using Scrapwalker.Game.Wording.Words;
using UnityEngine.UIElements;

namespace Scrapwalker.Game.UI.Widgets
{
    public sealed class LootView
    {
        private const string ItemGap = "　";

        private readonly VisualElement _root;
        private readonly RowList<VisualElement> _items;

        public LootView(VisualElement root)
        {
            _root = root;
            VisualElement list = root.Need<VisualElement>("loot-items");
            VisualTreeAsset item = list.TakeTemplate("loot-item-template");
            _items = new RowList<VisualElement>(list, () => item.Instantiate().Ignored(), row => row);
            root.Need<Label>("loot-suffix").text = " " + LogWords.Got;
        }

        public void Show(GameData data, IReadOnlyList<LootItem> items)
        {
            _root.Display(items.Count > 0);
            _items.SetCount(items.Count);
            for (int i = 0; i < items.Count; i++)
            {
                LootItem item = items[i];
                Label name = _items[i].Need<Label>("name");
                string separator = i > 0 ? ItemGap : string.Empty;
                name.text = separator + NameOf(data, item);
                name.SetQuality(item.Grade);
                _items[i].Need<Label>("count").text = " " + Texts.Times(item.Count);
            }
        }

        private static string NameOf(GameData data, LootItem item)
        {
            string name = item.Type.Name(data);
            string tier = item.Grade.Name();
            return tier.Length > 0 ? name + " " + tier : name;
        }
    }
}
