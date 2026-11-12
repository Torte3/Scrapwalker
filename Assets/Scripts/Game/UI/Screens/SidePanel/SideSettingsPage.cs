using Scrapwalker.Game.Audio;
using Scrapwalker.Game.UI.Widgets;
using Scrapwalker.Game.Wording.Words;
using UnityEngine;
using UnityEngine.UIElements;

namespace Scrapwalker.Game.UI.Screens.SidePanel
{
    internal sealed class SideSettingsPage : UIScreen
    {
        private readonly VolumeRow _bgm;
        private readonly VolumeRow _se;

        public SideSettingsPage(VisualElement root) : base(root)
        {
            _bgm = new VolumeRow(VolumeChannel.BGM, Q<Slider>("volume-bgm"), Q<Label>("volume-bgm-value"));
            _se = new VolumeRow(VolumeChannel.SE, Q<Slider>("volume-se"), Q<Label>("volume-se-value"));
            root.WriteLabel("volume-bgm-name", SideWords.VolumeBGM);
            root.WriteLabel("volume-se-name", SideWords.VolumeSE);
        }

        protected override void OnBind()
        {
            _bgm.Bind(App.Volumes);
            _se.Bind(App.Volumes);
        }

        private sealed class VolumeRow
        {
            private readonly VolumeChannel _channel;
            private readonly Slider _slider;
            private readonly Label _value;
            private VolumeSettings _volumes;

            public VolumeRow(VolumeChannel channel, Slider slider, Label value)
            {
                _channel = channel;
                _slider = slider;
                _value = value;
                _slider.RegisterValueChangedCallback(Slider_ValueChanged);
            }

            public void Bind(VolumeSettings volumes)
            {
                _volumes = volumes;
                float volume = volumes.Get(_channel);
                _slider.SetValueWithoutNotify(volume);
                ShowValue(volume);
            }

            private void Slider_ValueChanged(ChangeEvent<float> e)
            {
                _volumes.Set(_channel, e.newValue);
                ShowValue(e.newValue);
            }

            private void ShowValue(float volume)
            {
                _value.text = SideWords.VolumeValue(Mathf.RoundToInt(volume * 100f));
            }
        }
    }
}
