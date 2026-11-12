namespace Scrapwalker.Game.MainView
{
    public abstract class WorldPainter
    {
        protected WorldPainter(WorldScene scene)
        {
            Scene = scene;
        }

        protected WorldScene Scene { get; }
        protected CanvasStack Canvas => Scene.Canvas;
    }
}
