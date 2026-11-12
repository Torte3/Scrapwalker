namespace Scrapwalker.Core.Outpost.Grid
{
    public readonly struct GridPoint
    {
        public float X { get; }
        public float Y { get; }

        public GridPoint(float x, float y)
        {
            X = x;
            Y = y;
        }

        internal PointMemento CreateMemento()
        {
            return new PointMemento { X = X, Y = Y };
        }

        internal static GridPoint Restore(PointMemento memento)
        {
            return new GridPoint(memento.X, memento.Y);
        }

        public float StepsTo(GridPoint other)
        {
            return System.Math.Abs(other.X - X) + System.Math.Abs(other.Y - Y);
        }

        public override string ToString()
        {
            return "(" + X.ToString("0.0") + "," + Y.ToString("0.0") + ")";
        }
    }
}
