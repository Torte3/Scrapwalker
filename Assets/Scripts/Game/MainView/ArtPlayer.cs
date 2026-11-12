using Scrapwalker.Game.Definitions;
using Scrapwalker.Game.Look.Art;
using Scrapwalker.Game.Look.Art.Parts;
using Scrapwalker.Game.MainView.Effects;
using UnityEngine;

namespace Scrapwalker.Game.MainView
{
    public sealed class ArtPlayer : IArtStage
    {
        private readonly WorldScene _scene;
        private readonly ArtParts _parts;
        private readonly EffectsPainter _effects;
        private DrawBand _band;
        private ArtState _state;

        public ArtPlayer(WorldScene scene, ArtParts parts, EffectsPainter effects)
        {
            _scene = scene;
            _parts = parts;
            _effects = effects;
        }

        private CanvasStack Canvas => _scene.Canvas;

        public void Draw(ArtEntry entry, ArtState state, DrawBand band)
        {
            Draw(entry, null, state, band);
        }

        public void Draw(ArtEntry entry, ArtEntry overlay, ArtState state, DrawBand band)
        {
            _band = band;
            _state = state;
            ArtLayer[] layers = entry.Layers;
            foreach (ArtLayer layer in layers)
            {
                ArtLayer replacement = layer.Replacement;
                if (replacement != null && state.IsShown(replacement))
                {
                    DrawLayer(replacement);
                }
                else if (!IsReplacementIn(layers, layer))
                {
                    DrawLayer(layer);
                }
            }

            if (overlay == null)
            {
                return;
            }

            foreach (ArtLayer layer in overlay.Layers)
            {
                if (!IsReplacementIn(layers, layer))
                {
                    DrawLayer(layer);
                }
            }
        }

        public void DrawAboveFigures(ArtEntry overlay, ArtState state, DrawBand band)
        {
            _band = band;
            _state = state;
            foreach (ArtLayer layer in overlay.Layers)
            {
                foreach (ArtPart part in layer.Parts)
                {
                    if (part.DrawsAboveFigures && state.IsShown(layer))
                    {
                        part.Draw(this, state, layer);
                    }
                }
            }
        }

        private static bool IsReplacementIn(ArtLayer[] layers, ArtLayer layer)
        {
            foreach (ArtLayer other in layers)
            {
                if (other.Replacement == layer)
                {
                    return true;
                }
            }

            return false;
        }

        private void DrawLayer(ArtLayer layer)
        {
            if (!_state.IsShown(layer))
            {
                return;
            }

            ArtPart drawer = null;
            foreach (ArtPart part in layer.Parts)
            {
                part.Emit(this);
                if (part.DrawsAboveFigures)
                {
                    return;
                }

                if (part.DrawsItself)
                {
                    drawer = part;
                }
            }

            Canvas.Save();
            if (Place(layer))
            {
                if (drawer != null)
                {
                    drawer.Draw(this, _state, layer);
                }
                else
                {
                    PutLayer(layer, _state.FrameOf(layer));
                }
            }

            Canvas.Restore();
        }

        private bool Place(ArtLayer layer)
        {
            if (layer.Parent != null && !Place(layer.Parent))
            {
                return false;
            }

            foreach (ArtPart part in layer.Parts)
            {
                if (!part.Place(this, _state, layer))
                {
                    return false;
                }
            }

            return true;
        }

        private void PutLayer(ArtLayer layer, ArtFrame frame)
        {
            _parts.Put(layer, frame, _band, _state.TintOf(layer));
            Color? flash = _state.Flash;
            if (flash.HasValue && !layer.IsAdditive)
            {
                _parts.Flash(frame, _band, flash.Value);
            }
        }

        public void Save()
        {
            Canvas.Save();
        }

        public void Restore()
        {
            Canvas.Restore();
        }

        public void Translate(float x, float y)
        {
            Canvas.Translate(x, y);
        }

        public void Rotate(float angle)
        {
            Canvas.Rotate(angle);
        }

        public void RotateAbout(Vector2 pivot, float angle)
        {
            Canvas.RotateAbout(pivot, angle);
        }

        public void Scale(float sx, float sy)
        {
            Canvas.Scale(sx, sy);
        }

        public Vector2 FromCanvas(Vector2 p)
        {
            return Canvas.FromCanvas(p);
        }

        public void Put(ArtLayer layer, ArtFrame frame, Color tint)
        {
            _parts.Put(layer, frame, _band, tint);
        }

        public void Disc(float x, float y, float radius, Color tint)
        {
            _parts.Disc(x, y, radius, tint, _band);
        }

        public void Glow(float x, float y, float radiusX, float radiusY, Color tint)
        {
            _parts.Glow(x, y, radiusX, radiusY, tint, _band);
        }

        public void FillRect(float x, float y, float w, float h, Color tint)
        {
            _parts.FillRect(x, y, w, h, tint, _band);
        }

        public bool IsEvery(int frames)
        {
            return _scene.IsEvery(frames);
        }

        public void Emit(in SparkBurst burst, Vector2 at)
        {
            _effects.Emit(burst, at);
        }
    }
}
