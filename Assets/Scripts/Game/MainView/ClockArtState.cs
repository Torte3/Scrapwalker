using Scrapwalker.Game.Look.Art;

namespace Scrapwalker.Game.MainView
{
    public sealed class ClockArtState : ArtState
    {
        private readonly WorldScene _scene;

        public ClockArtState(WorldScene scene)
        {
            _scene = scene;
        }

        public override float Clock => _scene.ElapsedFrames;
    }
}
