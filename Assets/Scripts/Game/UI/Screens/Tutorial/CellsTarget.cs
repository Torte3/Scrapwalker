using Scrapwalker.Core.Outpost.Grid;
using UnityEngine;

namespace Scrapwalker.Game.UI.Screens.Tutorial
{
    public sealed class CellsTarget : TutorialTarget
    {
        private readonly CellRect _cells;

        public CellsTarget(CellRect cells)
        {
            _cells = cells;
        }

        public override Rect? AreaOn(TutorialScreen screen)
        {
            return screen.CellsRect(_cells);
        }

        public override bool IsShown => true;
    }
}
