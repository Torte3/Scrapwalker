using Scrapwalker.Game.Look;
using UnityEngine;

namespace Scrapwalker.Game.UI.Screens.WaveBanner
{
    public abstract class WaveBannerKind
    {
        public static readonly WaveBannerKind Warning = new WarningKind();
        public static readonly WaveBannerKind WaveClear = new WaveClearKind();
        public static readonly WaveBannerKind Cleared = new ClearedKind();
        public static readonly WaveBannerKind Launch = new LaunchKind();

        public abstract string USSName { get; }

        public virtual bool IsUrgent => false;

        public abstract float OpacityAt(float elapsedFrames);

        public virtual bool IsTextBlinkedOff(float elapsedFrames)
        {
            return false;
        }

        public virtual float ChargeAt(float elapsedFrames)
        {
            return 0f;
        }

        private abstract class FadingKind : WaveBannerKind
        {
            protected const float LifeFrames = 170f;
            private const float FadeInFrames = 8f;
            private const float FadeOutFrames = 20f;

            public override float OpacityAt(float elapsedFrames)
            {
                return Mathf.Min(Mathf.Min(1f, elapsedFrames / FadeInFrames), (LifeFrames - elapsedFrames) / FadeOutFrames);
            }
        }

        private sealed class WarningKind : FadingKind
        {
            private const float BlinkUntilFrames = 48f;
            private const float BlinkPeriodFrames = 12f;
            private const float BlinkOnFrames = 8f;

            public override string USSName => "warn";
            public override bool IsUrgent => true;

            public override bool IsTextBlinkedOff(float elapsedFrames)
            {
                return elapsedFrames < BlinkUntilFrames && Mathf.Repeat(elapsedFrames, BlinkPeriodFrames) > BlinkOnFrames;
            }
        }

        private sealed class WaveClearKind : FadingKind
        {
            public override string USSName => "held";
        }

        private sealed class LaunchKind : FadingKind
        {
            private const float ChargeFrames = 120f;

            public override string USSName => "launch";

            public override float ChargeAt(float elapsedFrames)
            {
                return Pen.Ease(Mathf.Clamp01(elapsedFrames / ChargeFrames));
            }
        }

        private sealed class ClearedKind : WaveBannerKind
        {
            private const float FadeInFrames = 12f;

            public override string USSName => "cleared";

            public override float OpacityAt(float elapsedFrames)
            {
                return Mathf.Min(1f, elapsedFrames / FadeInFrames);
            }
        }
    }
}
