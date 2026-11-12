using Scrapwalker.Core.Combat.Waves;
using Scrapwalker.Core.Root;
using Scrapwalker.Core.Travel;
using Scrapwalker.Game.App;
using Scrapwalker.Game.Look;
using Scrapwalker.Game.UI.Widgets;
using Scrapwalker.Game.Wording.Words;
using UnityEngine;
using UnityEngine.UIElements;

namespace Scrapwalker.Game.UI.Screens.WaveBanner
{
    public sealed class WaveBannerScreen : UIScreen
    {
        private const float FramesPerSecond = 60f;
        private const float OpenFrames = 14f;
        private const float TextsAfterOpen = 0.6f;

        private readonly VisualElement _banner;
        private readonly VisualElement _band;
        private readonly VisualElement _texts;
        private readonly VisualElement _charge;
        private readonly Label _label;
        private readonly Label _title;
        private readonly BannerStripes _stripes;
        private readonly ShownValue<bool> _textsShown = new ShownValue<bool>();

        private WaveBannerKind _kind;
        private float _elapsedFrames;

        public WaveBannerScreen(VisualElement root) : base(root)
        {
            _banner = Q("wave-banner");
            _band = Q("band");
            _texts = Q("texts");
            _charge = Q("charge");
            _label = Q<Label>("label");
            _title = Q<Label>("title");
            _stripes = new BannerStripes(root);
        }

        protected override void OnBind()
        {
            Clear();
        }

        public void Listen(GameEventListeners listeners)
        {
            listeners.On<WaveStarted>(OnWaveStarted);
            listeners.On<WaveCleared>(OnWaveCleared);
            listeners.On<Landed>(_ => Clear());
        }

        private void OnWaveStarted(WaveStarted started)
        {
            if (started.IsTakeoffWave)
            {
                ShowLaunch();
            }
            else
            {
                ShowWarning(started.Number, started.Count);
            }
        }

        private void OnWaveCleared(WaveCleared cleared)
        {
            if (cleared.IsTakeoffWave)
            {
                return;
            }

            if (cleared.LeavesRuin)
            {
                ShowCleared();
            }
            else
            {
                ShowWaveClear(cleared.Number, cleared.Count);
            }
        }

        public void Clear()
        {
            _kind = null;
            _banner.Display(false);
        }

        private void ShowWarning(int number, int count)
        {
            Show(WaveBannerKind.Warning, BannerWords.Warning(number), BannerWords.WarningLabel(number, count));
        }

        private void ShowLaunch()
        {
            Show(WaveBannerKind.Launch, BannerWords.Launch, BannerWords.LaunchLabel);
        }

        private void ShowWaveClear(int number, int count)
        {
            Show(WaveBannerKind.WaveClear, BannerWords.WaveClear(number), BannerWords.WaveClearLabel(number, count));
        }

        private void ShowCleared()
        {
            Show(WaveBannerKind.Cleared, BannerWords.Cleared, BannerWords.ClearedLabel);
        }

        private void Show(WaveBannerKind kind, string title, string label)
        {
            _kind = kind;
            _elapsedFrames = 0f;
            _title.text = title;
            _label.text = label;
            _banner.SetVariant("wave-banner--", kind.USSName);
            _textsShown.Reset();
            Advance(0f);
            _banner.Display(true);
        }

        public void Refresh()
        {
            if (_kind == null)
            {
                return;
            }

            if (World.Journey.IsTraveling)
            {
                Clear();
                return;
            }

            bool isAISpeaking = Session.Speaker.ShowsTalk;
            _banner.Display(!isAISpeaking);
            if (isAISpeaking)
            {
                return;
            }

            Advance(App.GameDelta * FramesPerSecond);
        }

        private void Advance(float frames)
        {
            _elapsedFrames += frames;
            float opacity = _kind.OpacityAt(_elapsedFrames);
            if (opacity <= 0f && _elapsedFrames > 0f)
            {
                Clear();
                return;
            }

            float open = Pen.Ease(Mathf.Clamp01(_elapsedFrames / OpenFrames));
            _banner.style.opacity = Mathf.Max(0f, opacity);
            _band.style.scale = new Scale(new Vector3(1f, open, 1f));
            if (_kind.IsUrgent)
            {
                _stripes.Advance(_elapsedFrames);
            }

            _charge.style.scale = new Scale(new Vector3(_kind.ChargeAt(_elapsedFrames), 1f, 1f));

            bool showsTexts = open > TextsAfterOpen && !_kind.IsTextBlinkedOff(_elapsedFrames);
            if (_textsShown.Change(showsTexts))
            {
                _texts.Show(showsTexts);
            }
        }
    }
}
