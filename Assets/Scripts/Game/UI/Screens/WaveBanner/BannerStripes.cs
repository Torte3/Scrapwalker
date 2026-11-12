using Scrapwalker.Game.UI.Widgets;
using UnityEngine;
using UnityEngine.UIElements;

namespace Scrapwalker.Game.UI.Screens.WaveBanner
{
    internal sealed class BannerStripes
    {
        private const float EdgePeriod = 28.8f;
        private const float BandPeriod = 24f;
        private const float EdgeSpeed = 1.2f;
        private const float BandSpeed = 2f;
        private const float GlowRadiansPerFrame = 0.22f;

        private readonly VisualElement _edgeGlow;
        private readonly VisualElement _edgeTop;
        private readonly VisualElement _edgeBottom;
        private readonly VisualElement _bandTop;
        private readonly VisualElement _bandBottom;

        public BannerStripes(VisualElement root)
        {
            _edgeGlow = root.Need<VisualElement>("edge-glow");
            _edgeTop = root.Need<VisualElement>("edge-top-stripes");
            _edgeBottom = root.Need<VisualElement>("edge-bottom-stripes");
            _bandTop = root.Need<VisualElement>("band-top-stripes");
            _bandBottom = root.Need<VisualElement>("band-bottom-stripes");
        }

        public void Advance(float elapsedFrames)
        {
            _edgeGlow.style.opacity = 0.5f + 0.5f * Mathf.Sin(elapsedFrames * GlowRadiansPerFrame);
            Slide(_edgeTop, elapsedFrames * EdgeSpeed, EdgePeriod);
            Slide(_edgeBottom, -elapsedFrames * EdgeSpeed, EdgePeriod);
            Slide(_bandTop, elapsedFrames * BandSpeed, BandPeriod);
            Slide(_bandBottom, -elapsedFrames * BandSpeed, BandPeriod);
        }

        private static void Slide(VisualElement stripes, float offset, float period)
        {
            stripes.style.translate = new Translate(Mathf.Repeat(offset, period), 0f);
        }
    }
}
