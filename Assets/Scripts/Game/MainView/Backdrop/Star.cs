namespace Scrapwalker.Game.MainView.Backdrop
{
    public readonly struct Star
    {
        public Star(float x, float y, float radius, float alpha)
        {
            X = x;
            Y = y;
            Radius = radius;
            Alpha = alpha;
        }

        public float X { get; }
        public float Y { get; }
        public float Radius { get; }
        public float Alpha { get; }
    }
}
