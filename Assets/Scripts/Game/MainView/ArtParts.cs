using Scrapwalker.Game.Look;
using Scrapwalker.Game.Look.Art;
using UnityEngine;

namespace Scrapwalker.Game.MainView
{
    public sealed class ArtParts
    {
        private readonly SpriteBatch _batch;
        private readonly CanvasStack _canvas;

        public ArtParts(SpriteBatch batch, ArtCatalog art, CanvasStack canvas)
        {
            _batch = batch;
            Art = art;
            _canvas = canvas;
        }

        public ArtCatalog Art { get; }

        public void Disc(float x, float y, float radius, Color tint, DrawBand band)
        {
            Stretch(Art.Effects.Disc, x, y, radius * 2f, radius * 2f, tint, band);
        }

        public void Glow(float x, float y, float radiusX, float radiusY, Color tint, DrawBand band)
        {
            Stretch(Art.Effects.Glow, x, y, radiusX * 2f, radiusY * 2f, tint, band);
        }

        public void Shadow(float x, float y, float radiusX, float radiusY, Color tint, DrawBand band)
        {
            Stretch(Art.Effects.Shadow, x, y, radiusX * 2f, radiusY * 2f, tint, band);
        }

        public void FillRect(float x, float y, float w, float h, Color tint, DrawBand band)
        {
            Stretch(Art.Effects.White, x, y, w, h, tint, band);
        }

        public void Rise(float x0, float x1, float groundY, float h, Color tint, DrawBand band)
        {
            Stretch(Art.Effects.Rise, x0, groundY - h, x1 - x0, h, tint, band);
        }

        public void StretchWidth(ArtLayer layer, float x, float y, float w, Color tint, DrawBand band)
        {
            Sprite sprite = layer.First?.Sprite;
            if (sprite != null)
            {
                Stretch(layer, x, y, w, sprite.bounds.size.y, tint, band);
            }
        }

        private void Stretch(ArtLayer layer, float x, float y, float w, float h, Color tint, DrawBand band)
        {
            Sprite sprite = layer.First?.Sprite;
            if (sprite == null)
            {
                return;
            }

            Vector2 size = sprite.bounds.size;
            _canvas.Save();
            _canvas.Translate(x, y);
            _canvas.Scale(w / size.x, h / size.y);
            Put(layer, layer.First, band, tint);
            _canvas.Restore();
        }

        public void Put(ArtLayer layer, DrawBand band)
        {
            Put(layer, band, Color.white);
        }

        public void Put(ArtLayer layer, DrawBand band, Color tint)
        {
            if (layer != null)
            {
                Put(layer, layer.First, band, tint);
            }
        }

        public void PutAt(ArtLayer layer, Vector2 at, float angle, DrawBand band)
        {
            _canvas.Save();
            _canvas.Translate(at.x, at.y);
            _canvas.Rotate(angle);
            Put(layer, band);
            _canvas.Restore();
        }

        public void PutBetween(ArtLayer layer, ArtFrame frame, Vector2 from, Vector2 to, float artLength, float thickness, DrawBand band, Color tint)
        {
            Vector2 d = to - from;
            _canvas.Save();
            _canvas.Translate(from.x, from.y);
            _canvas.Rotate(Pen.Direction(from, to));
            _canvas.Scale(d.magnitude / artLength, thickness);
            Put(layer, frame, band, tint);
            _canvas.Restore();
        }

        public void Put(ArtLayer layer, ArtFrame frame, DrawBand band, Color tint)
        {
            _batch.Draw(layer, frame, band, tint);
        }

        public void Flash(ArtFrame frame, DrawBand band, Color tint)
        {
            _batch.DrawFlash(frame, band, tint);
        }
    }
}
