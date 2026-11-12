namespace Scrapwalker.Game.MainView.Compose
{
    public struct TravelVisual
    {
        public bool HasRoute;
        public float RouteFrames;
        public float RouteProgress;

        public static float RouteProgressOf(int remainingTicks, int totalTicks, float tickShare)
        {
            return 1f - System.Math.Max(0f, remainingTicks - tickShare) / totalTicks;
        }
    }
}
