using Scrapwalker.Game.Look.Art;
using Scrapwalker.Game.Look.Art.Parts;
using UnityEngine;
using static Scrapwalker.Game.MainView.WorldScene;

namespace Scrapwalker.Game.MainView.Outpost
{
    // 胴の絵は 9×5 マスで書き出してある。GridColumns・GridRows を変えたら書き出し直す
    public sealed class HullPainter : WorldPainter
    {
        private const float RadarX = 336f;

        private readonly ArtParts _parts;
        private readonly ArtPlayer _player;
        private readonly ClockArtState _state;

        public HullPainter(WorldScene scene, ArtParts parts, ArtPlayer player)
            : base(scene)
        {
            _parts = parts;
            _player = player;
            _state = new ClockArtState(scene);
        }

        public void Draw(int radarLook)
        {
            _player.Draw(_parts.Art.Outpost.Hull, _state, DrawBand.Hull);
            ArtEntry radar = _parts.Art.Outpost.Radars[radarLook];
            Vector2 stand = radar.Find<ArtRig>().At;
            Canvas.Save();
            Canvas.Translate(HullLeft + RadarX - stand.x, Scene.HullTop - Armor - stand.y);
            _player.Draw(radar, _state, DrawBand.Hull);
            Canvas.Restore();
        }
    }
}
