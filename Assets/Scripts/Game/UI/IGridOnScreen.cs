using Scrapwalker.Core.Outpost.Grid;
using UnityEngine;

namespace Scrapwalker.Game.UI
{
    public interface IGridOnScreen
    {
        Vector2 PointOf(float column, float row);

        Vector2 TopCenterOf(CellRect rect);

        int EnemyAt(Vector2 screen);

        Cell? CellAt(Vector2 screen, CellRect grid);
    }
}
