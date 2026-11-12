using UnityEngine;
using UnityEngine.Audio;

namespace Scrapwalker.Game.Audio.Players
{
    public sealed class SEOutput
    {
        private const float MaxPitch = 3f;
        private const float MinShift = 0.5f;
        private const float MaxShift = 2f;

        private readonly AudioMixerGroup _normal;
        private readonly AudioMixerGroup _shifted;
        private readonly PitchShiftParameters _shift;
        private readonly float _rate;
        private float _timeScale = -1f;

        public SEOutput(AudioMixerGroup normal, AudioMixerGroup shifted, PitchShiftParameters shift, float rate)
        {
            _normal = normal;
            _shifted = shifted;
            _shift = shift;
            _rate = rate;
            Follow(1f);
        }

        public AudioMixerGroup GroupAt(float timeScale)
        {
            return PitchAt(1f, timeScale) != 1f ? _shifted : _normal;
        }

        public float PitchAt(float pitch, float timeScale)
        {
            return Mathf.Min(MaxPitch, pitch * _rate * Mathf.Max(1f, timeScale));
        }

        public void Follow(float timeScale)
        {
            if (timeScale == _timeScale)
            {
                return;
            }

            _timeScale = timeScale;
            float shift = 1f / PitchAt(1f, timeScale);
            float first = Mathf.Clamp(shift, MinShift, MaxShift);
            _shifted.audioMixer.SetFloat(_shift.First, first);
            _shifted.audioMixer.SetFloat(_shift.Second, shift / first);
        }
    }
}
