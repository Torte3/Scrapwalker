using System.Collections.Generic;
using Scrapwalker.Core.Outpost.Bots;
using Scrapwalker.Game.UI.Widgets;
using UnityEngine.UIElements;

namespace Scrapwalker.Game.UI.Screens.SidePanel
{
    internal sealed class SideBotsPage : UIScreen
    {
        private readonly Label _count;
        private readonly RowList<SideBotRow> _rows;
        private readonly ShownValue<int> _countShown = new ShownValue<int>();

        public SideBotsPage(VisualElement root, VisualTreeAsset rowTemplate) : base(root)
        {
            _count = Q<Label>("bot-count");
            _rows = new RowList<SideBotRow>(Q<ScrollView>("bot-list"), () => new SideBotRow(rowTemplate.Instantiate()), row => row.Element);
        }

        protected override void OnBind()
        {
            _countShown.Reset();
            _rows.SetCount(0);
        }

        public void Refresh()
        {
            if (_countShown.Change(World.Bots.Count))
            {
                _count.text = World.Bots.Count.ToString();
            }

            IReadOnlyList<Bot> bots = World.Bots;
            _rows.SetCount(bots.Count);
            for (int i = 0; i < bots.Count; i++)
            {
                _rows[i].Refresh(Data, World, bots[i]);
            }
        }
    }
}
