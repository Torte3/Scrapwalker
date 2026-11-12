using Scrapwalker.Core.Outpost.Grid;
using Scrapwalker.Game.MainView;
using Scrapwalker.Game.UI;
using UnityEngine;

namespace Scrapwalker.Game.Cameras
{
    public sealed class GridOnScreen : IGridOnScreen
    {
        private const float FieldDepth = 10f;

        private readonly Camera _camera;
        private readonly WorldView _view;

        public GridOnScreen(Camera camera, WorldView view)
        {
            _camera = camera;
            _view = view;
        }

        public Vector2 PointOf(float column, float row)
        {
            return _camera.WorldToScreenPoint(_view.UnitsOfCell(column, row));
        }

        public Vector2 TopCenterOf(CellRect rect)
        {
            return PointOf(rect.Column + rect.Width * 0.5f, rect.EndRow);
        }

        public int EnemyAt(Vector2 screen)
        {
            Vector3 units = _camera.ScreenToWorldPoint(new Vector3(screen.x, screen.y, FieldDepth));
            return _camera.pixelRect.Contains(screen) ? _view.EnemyAt(WorldScene.PixelsOfUnits(units)) : 0;
        }

        public Cell? CellAt(Vector2 screen, CellRect grid)
        {
            Vector2 cellPoint = _view.CellOfUnits(_camera.ScreenToWorldPoint(new Vector3(screen.x, screen.y, FieldDepth)));
            var cell = new Cell(Mathf.FloorToInt(cellPoint.x), Mathf.FloorToInt(cellPoint.y));
            return _camera.pixelRect.Contains(screen) && grid.Covers(cell) ? cell : (Cell?)null;
        }
    }
}
