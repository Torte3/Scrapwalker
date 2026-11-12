using Scrapwalker.Core.Outpost;
using Scrapwalker.Core.Outpost.Facilities.Storage;
using Scrapwalker.Core.Root;
using Scrapwalker.Game.UI.Widgets.Materials;
using Scrapwalker.Game.UI.Widgets;
using Scrapwalker.Game.Wording.Words;
using UnityEngine.UIElements;

namespace Scrapwalker.Game.UI.Screens.FacilityPanel
{
    internal sealed class FacilityStorePanel : UIScreen
    {
        private readonly Meter _capacity;
        private readonly StockList _stocks;
        private readonly VisualElement _empty;
        private readonly ShownValue<(World, Facility)> _shape = new ShownValue<(World, Facility)>();

        public FacilityStorePanel(VisualElement root, FacilityTemplates templates) : base(root)
        {
            _capacity = new Meter(Q<Label>("capacity"), Q("capacity-fill"), RoomWords.CapacityValue);
            _stocks = new StockList(Q("stock-lines"), () => templates.Stock.Instantiate(), onlyHeld: true);
            _empty = Q("store-empty");
            root.WriteLabel("store-empty", RoomWords.EmptyStore);
            root.WriteLabel("store-title", RoomWords.SectionStore);
            root.WriteLabel("capacity-key", RoomWords.Capacity);
        }

        public void Reset()
        {
            _shape.Reset();
        }

        public void Refresh(Facility f)
        {
            if (_shape.Change((World, f)))
            {
                Rebuild(f);
            }

            Inventory inventory = f.Inventory;
            if (inventory == null)
            {
                return;
            }

            _capacity.Set(inventory.MaterialCount, inventory.Capacity);
            _empty.Display(_stocks.Refresh(inventory.Material) == 0);
        }

        private void Rebuild(Facility f)
        {
            Root.Display(f.Inventory != null);
            if (f.Inventory == null)
            {
                return;
            }

            _capacity.Reset();
            _stocks.Rebuild(Data, World.Definitions.Materials);
        }
    }
}
