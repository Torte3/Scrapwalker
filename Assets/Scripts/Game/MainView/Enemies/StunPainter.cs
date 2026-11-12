using Scrapwalker.Game.Look.Art;
using UnityEngine;

namespace Scrapwalker.Game.MainView.Enemies
{
    public sealed class StunPainter : WorldPainter
    {
        private const float ArtWidth = 60f;
        private const float ArtHeight = 50f;

        private readonly ArtParts _parts;

        public StunPainter(WorldScene scene, ArtParts parts)
            : base(scene)
        {
            _parts = parts;
        }

        public void Draw(EnemyLook look, float clock, DrawBand band)
        {
            Canvas.Save();
            Canvas.Translate(look.Middle, 0f);
            Canvas.Scale(look.Size.x / ArtWidth, look.Size.y / ArtHeight);
            _parts.Put(_parts.Art.Effects.StunGlow, band);
            ArtLayer sparks = _parts.Art.Effects.StunSparks;
            _parts.Put(sparks, sparks.At(clock), band, Color.white);

            Canvas.Restore();
        }
    }
}
