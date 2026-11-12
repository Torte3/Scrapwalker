using UnityEngine;
using UnityEngine.Audio;

namespace Scrapwalker.Game.Audio
{
    public sealed class VolumeSettings
    {
        private const float MinDecibels = -80f;
        private const float Default = 1f;

        private readonly AudioMixer _mixer;
        private readonly SoundTable _table;

        public VolumeSettings(SoundTable table)
        {
            _mixer = table.BGMGroup.audioMixer;
            _table = table;
        }

        public float Get(VolumeChannel channel)
        {
            return PlayerPrefs.GetFloat(KeyOf(channel), Default);
        }

        public void Set(VolumeChannel channel, float volume)
        {
            PlayerPrefs.SetFloat(KeyOf(channel), Mathf.Clamp01(volume));
            PlayerPrefs.Save();
            Apply(channel);
        }

        public void ApplyAll()
        {
            Apply(VolumeChannel.BGM);
            Apply(VolumeChannel.SE);
        }

        private void Apply(VolumeChannel channel)
        {
            float volume = Get(channel);
            _mixer.SetFloat(_table.VolumeParameter(channel), volume > 0f ? Mathf.Max(MinDecibels, 20f * Mathf.Log10(volume)) : MinDecibels);
        }

        private static string KeyOf(VolumeChannel channel)
        {
            return "Scrapwalker.Volume." + channel;
        }
    }
}
