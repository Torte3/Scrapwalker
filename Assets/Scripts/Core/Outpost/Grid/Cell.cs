namespace Scrapwalker.Core.Outpost.Grid
{
    public readonly struct Cell : System.IEquatable<Cell>
    {
        public int Column { get; }
        public int Row { get; }

        public Cell(int column, int row)
        {
            Column = column;
            Row = row;
        }

        internal CellMemento CreateMemento()
        {
            return new CellMemento { Column = Column, Row = Row };
        }

        internal static Cell Restore(CellMemento memento)
        {
            return new Cell(memento.Column, memento.Row);
        }

        public bool Equals(Cell other)
        {
            return Column == other.Column && Row == other.Row;
        }

        public override bool Equals(object value)
        {
            return value is Cell other && Equals(other);
        }

        public override int GetHashCode()
        {
            return Column * 397 ^ Row;
        }

        internal string LogText => "@" + this;

        public override string ToString()
        {
            return "(" + Column + "," + Row + ")";
        }

        public static bool operator ==(Cell a, Cell b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(Cell a, Cell b)
        {
            return !a.Equals(b);
        }
    }
}
