using Scrapwalker.Game.Look;
using UnityEngine;

namespace Scrapwalker.Game.MainView.Enemies
{
    public sealed class BarrierPainter
    {
        private const float Gap = 6f;
        private const float FilmWidth = 10f;
        private const float LineWidth = 2f;
        private const float FlashGrow = 8f;
        private const float HalfAngle = 1.15f;

        private readonly ArcMesh _arcs;

        public BarrierPainter(ArcMesh arcs)
        {
            _arcs = arcs;
        }

        public void Draw(EnemyFigure figure, EnemyLook look, Vector2 center)
        {
            float cx = center.x;
            float cy = center.y;
            float inner = Mathf.Max(look.Size.x, look.Size.y) * 0.5f + 4f;
            float from = Mathf.PI - HalfAngle;
            float to = Mathf.PI + HalfAngle;
            float outer = inner + (figure.BarrierLayers + 1) * Gap;
            _arcs.ClipRect(cx - outer - FlashGrow - 10f, cy - outer - FlashGrow - 10f, outer + FlashGrow + 10f, WorldScene.Ground - (cy - outer - FlashGrow - 10f));
            for (int i = 0; i < figure.BarrierLayers; i++)
            {
                float r = inner + i * Gap;
                _arcs.StrokeArc(cx, cy, r, from, to, FilmWidth, Palette.BarrierFilm, 20);
                _arcs.StrokeArc(cx, cy, r, from, to, LineWidth, Palette.BarrierLayer, 20);
            }

            if (figure.BarrierFlash > 0f)
            {
                float r = inner + figure.BarrierLayers * Gap + (1f - figure.BarrierFlash) * FlashGrow;
                _arcs.StrokeArc(cx, cy, r, from, to, LineWidth + 3f * figure.BarrierFlash, Palette.BarrierFlash.WithAlpha(Palette.BarrierFlash.a * figure.BarrierFlash), 20);
            }

            _arcs.Unclip();
        }
    }
}
