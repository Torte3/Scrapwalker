using UnityEngine;

namespace Scrapwalker.Game.MainView
{
    public readonly struct WorldBar
    {
        private readonly WorldUI _ui;
        private readonly Rect _rect;

        public WorldBar(WorldUI ui, Rect rect)
        {
            _ui = ui;
            _rect = rect;
        }

        public void Back(Color color, float border)
        {
            _ui.FillRect(_rect.x - border, _rect.y - border, _rect.width + border * 2f, _rect.height + border * 2f, color);
        }

        public void Edge(Color color, float border, float lineWidth)
        {
            _ui.StrokeRect(_rect.x - border, _rect.y - border, _rect.width + border * 2f, _rect.height + border * 2f, lineWidth, color);
        }

        public void Fill(float share, Color color)
        {
            Fill(0f, share, color);
        }

        public void Fill(float from, float to, Color color)
        {
            float start = _rect.width * Mathf.Clamp01(from);
            float end = _rect.width * Mathf.Clamp01(to);
            _ui.FillRect(_rect.x + start, _rect.y, end - start, _rect.height, color);
        }
    }
}
