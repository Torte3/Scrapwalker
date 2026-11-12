using Scrapwalker.Game.Look.Art;
using UnityEngine;

namespace Scrapwalker.Game.MainView.Bots
{
    internal sealed class BotArtState : ArtState
    {
        private float _clock;
        private int? _step;

        public bool ShowsLights { get; set; }

        public override float Clock => _clock;

        public void Begin(float clock, int? step)
        {
            _clock = clock;
            _step = step;
        }

        public override bool IsShown(ArtLayer layer)
        {
            return layer.IsLight == ShowsLights && base.IsShown(layer);
        }

        public override ArtFrame FrameOf(ArtLayer layer)
        {
            return _step.HasValue ? layer.ByValue(_step.Value) : layer.At(_clock);
        }

        public override Color TintOf(ArtLayer layer)
        {
            return new Color(1f, 1f, 1f, layer.Alpha(_clock));
        }
    }
}
