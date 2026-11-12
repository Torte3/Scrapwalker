namespace Scrapwalker.Core.Outpost.Grid
{
    public readonly struct CellRect
    {
        public Cell Origin { get; }
        public int Width { get; }
        public int Height { get; }

        public CellRect(Cell origin, int width, int height)
        {
            Origin = origin;
            Width = width;
            Height = height;
        }

        public int Column => Origin.Column;
        public int Row => Origin.Row;
        public int EndColumn => Column + Width;
        public int EndRow => Row + Height;

        public bool Covers(Cell cell)
        {
            return cell.Column >= Column && cell.Column < EndColumn && cell.Row >= Row && cell.Row < EndRow;
        }

        public bool Overlaps(CellRect other)
        {
            return Column < other.EndColumn && other.Column < EndColumn && Row < other.EndRow && other.Row < EndRow;
        }

        public bool FitsIn(int columns, int rows)
        {
            return Column >= 0 && Row >= 0 && EndColumn <= columns && EndRow <= rows;
        }
    }
}
