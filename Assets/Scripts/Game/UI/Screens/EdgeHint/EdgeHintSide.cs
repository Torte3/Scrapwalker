using Scrapwalker.Game.UI.Widgets;
using UnityEngine;
using UnityEngine.UIElements;

namespace Scrapwalker.Game.UI.Screens.EdgeHint
{
    internal sealed class EdgeHintSide
    {
        private const int Chevrons = 2;
        private const float FadeOpacity = 0.42f;
        private const float FadeOpacityByPull = 0.18f;
        private const float ChevronSize = 14f;
        private const float ChevronSizeByPull = 6f;
        private const float StrokeOverhang = 3.5f;
        private const float TipInset = 26f;
        private const float TipPulse = 4f;
        private const float SecondChevronFade = 0.55f;

        private readonly VisualElement _root;
        private readonly VisualElement _fade;
        private readonly VisualElement[] _chevrons = new VisualElement[Chevrons];
        private readonly VisualElement[] _strokes = new VisualElement[Chevrons * 2];
        private readonly float _inward;
        private readonly ShownValue<bool> _shown = new ShownValue<bool>();
        private float _opacity;

        public EdgeHintSide(VisualElement root, string name, float inward)
        {
            _root = root.Need<VisualElement>(name);
            _fade = root.Need<VisualElement>(name + "-fade");
            for (int i = 0; i < Chevrons; i++)
            {
                _chevrons[i] = root.Need<VisualElement>(name + "-chevron-" + i);
                _strokes[i * 2] = root.Need<VisualElement>(name + "-stroke-" + i + "-a");
                _strokes[i * 2 + 1] = root.Need<VisualElement>(name + "-stroke-" + i + "-b");
            }

            _inward = inward;
        }

        public bool IsScrollable { get; set; }

        public float Pull { get; set; }

        public void Reset()
        {
            IsScrollable = false;
            Pull = 0f;
            _opacity = 0f;
            _shown.Reset();
        }

        public void Refresh(float fadeStep, float pulse)
        {
            _opacity = Mathf.MoveTowards(_opacity, IsScrollable ? 1f : 0f, fadeStep);
            bool isShown = _opacity > 0f;
            if (_shown.Change(isShown))
            {
                _root.Show(isShown);
            }

            if (!isShown)
            {
                return;
            }

            _root.style.opacity = _opacity;
            _fade.style.opacity = FadeOpacity + FadeOpacityByPull * Pull;
            float size = ChevronSize + ChevronSizeByPull * Pull;
            float alpha = 0.45f + 0.25f * pulse * (1f - Pull) + 0.55f * Pull;
            float length = size * Mathf.Sqrt(2f) + StrokeOverhang;
            for (int i = 0; i < Chevrons; i++)
            {
                float tip = TipInset - TipPulse * pulse + i * size * 0.75f - size * 0.5f;
                _chevrons[i].style.translate = new Translate(_inward * tip, 0f);
                _chevrons[i].style.opacity = alpha * Mathf.Pow(SecondChevronFade, i);
                _strokes[i * 2].style.width = length;
                _strokes[i * 2 + 1].style.width = length;
            }
        }
    }
}
