using Scrapwalker.Core.Outpost.Grid;
using UnityEngine;
using static Scrapwalker.Game.MainView.WorldScene;

namespace Scrapwalker.Game.MainView.Compose
{
    public readonly struct FacilityRect
    {
        public const float FrameInset = 2f;

        public readonly int Column;
        public readonly int Row;
        public readonly int Width;
        public readonly int Height;

        public FacilityRect(int column, int row, int width, int height)
        {
            Column = column;
            Row = row;
            Width = width;
            Height = height;
        }

        public FacilityRect(CellRect cells)
            : this(cells.Column, cells.Row, cells.Width, cells.Height)
        {
        }

        public float Left => CellLeft(Column);
        public float Top => FloorY(Row + Height);
        public float Floor => FloorY(Row);
        public float PixelWidth => Width * CellSize;
        public float PixelHeight => Height * CellSize;
        public float CenterX => Left + PixelWidth * 0.5f;

        public Rect Inner => Inset(FrameInset);

        public FacilityCellWalk Cells => new FacilityCellWalk(this);

        public Rect Inset(float pixels)
        {
            return new Rect(Left + pixels, Top + pixels, PixelWidth - pixels * 2f, PixelHeight - pixels * 2f);
        }
    }
}
