using System.Collections.Generic;
using Scrapwalker.Core.Outpost.Floors;
using Scrapwalker.Core.Outpost.Grid;
using Scrapwalker.Game.Look;
using Scrapwalker.Game.MainView.Effects;
using TMPro;
using UnityEngine;
using static Scrapwalker.Game.MainView.WorldScene;

namespace Scrapwalker.Game.MainView.Outpost
{
    public sealed class PilePainter : WorldPainter
    {
        private const float IconScale = 0.7f;
        private const float IconHeight = 18f;

        private readonly ItemIcons _icons;
        private readonly WorldUI _ui;

        public PilePainter(WorldScene scene, ItemIcons icons, WorldUI ui)
            : base(scene)
        {
            _icons = icons;
            _ui = ui;
        }

        public void Draw(IReadOnlyList<Pile> piles)
        {
            for (int i = 0; i < piles.Count; i++)
            {
                Pile pile = piles[i];
                Cell cell = pile.Slot.Cell;
                float x = CellLeft(cell.Column) + CellSize * (pile.Slot.Index > 0 ? 0.72f : 0.28f);
                float floor = FloorY(cell.Row);
                _icons.DrawOnFloor(pile.Type, x, floor, IconScale, DrawBand.Piles);
                if (pile.Count > 1)
                {
                    _ui.Text(x, floor - IconHeight, pile.Count.ToString(), 10f, Palette.PileCount, FontStyles.Bold, TextAlignmentOptions.Bottom);
                }
            }
        }
    }
}
