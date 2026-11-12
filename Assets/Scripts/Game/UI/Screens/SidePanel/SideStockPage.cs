using System;
using Scrapwalker.Core.Items;
using Scrapwalker.Core.Root;
using Scrapwalker.Game.UI.Widgets.Materials;
using Scrapwalker.Game.UI.Widgets;
using Scrapwalker.Game.Wording.Words;
using UnityEngine.UIElements;
using MaterialDefinition = Scrapwalker.Core.Items.MaterialDefinition;

namespace Scrapwalker.Game.UI.Screens.SidePanel
{
    internal sealed class SideStockPage : UIScreen
    {
        private readonly Meter _storeUsage;
        private readonly StockList _stocks;
        private readonly Func<MaterialDefinition, Quality, int> _countHeld;

        public SideStockPage(VisualElement root, VisualTreeAsset cell) : base(root)
        {
            _storeUsage = new Meter(Q<Label>("store-usage"), null, SideWords.StoreUsage);
            _stocks = new StockList(Q<ScrollView>("stock-list"), () => StockCell(cell));
            _countHeld = (material, quality) => World.Ledger.HeldMaterial(material, QualityFilter.Exactly(quality));
        }

        protected override void OnBind()
        {
            _storeUsage.Reset();
            _stocks.Rebuild(Data, World.Definitions.Materials);
        }

        public void Refresh()
        {
            _storeUsage.Set(World.Stock.MaterialCount, World.Stock.Capacity);

            _stocks.Refresh(_countHeld);
        }

        private static VisualElement StockCell(VisualTreeAsset cell)
        {
            TemplateContainer instance = cell.Instantiate();
            instance.AddToClassList("side-stock-cell");
            return instance;
        }
    }
}
