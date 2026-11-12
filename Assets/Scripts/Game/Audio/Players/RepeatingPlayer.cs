using Scrapwalker.Game.Audio.Sounds;
using UnityEngine;

namespace Scrapwalker.Game.Audio.Players
{
    public sealed class RepeatingPlayer
    {
        private const float FadeSeconds = 0.15f;

        private readonly RepeatingSound _sound;
        private readonly SoundPlayer _accents;
        private readonly SEOutput _output;
        private readonly AudioSource _loop;
        private float _fade;
        private float _untilAccent;
        private float _pitch = 1f;
        private float _timeScale = 1f;
        private bool _isPaused;

        public RepeatingPlayer(RepeatingSound sound, Transform parent, SEOutput output, SoundPlayer accents)
        {
            _sound = sound;
            _accents = accents;
            _output = output;
            _loop = parent.gameObject.AddComponent<AudioSource>();
            _loop.playOnAwake = false;
            _loop.loop = true;
            _loop.outputAudioMixerGroup = output.GroupAt(_timeScale);
        }

        public void Tick(bool isOn, float deltaTime)
        {
            SoundCue loop = _sound.Loop;
            if (isOn && !loop.IsEmpty && !_loop.isPlaying)
            {
                _loop.clip = loop.Clip;
                _pitch = 1f + Random.Range(-loop.PitchJitter, loop.PitchJitter);
                _loop.pitch = _output.PitchAt(_pitch, _timeScale);
                _loop.Play();
                _untilAccent = NextAccent();
            }

            _fade = Mathf.MoveTowards(_fade, isOn ? 1f : 0f, deltaTime / FadeSeconds);
            _loop.volume = loop.Volume * _fade;
            if (_fade <= 0f && _loop.isPlaying)
            {
                _loop.Stop();
            }

            if (!isOn || _sound.Accent.IsEmpty)
            {
                return;
            }

            _untilAccent -= deltaTime * _timeScale;
            if (_untilAccent <= 0f)
            {
                _accents.Play(_sound.Accent);
                _untilAccent = NextAccent();
            }
        }

        public void SetTimeScale(float scale)
        {
            _timeScale = scale;
            if (scale <= 0f)
            {
                if (_loop.isPlaying)
                {
                    _loop.Pause();
                    _isPaused = true;
                }

                return;
            }

            _loop.outputAudioMixerGroup = _output.GroupAt(scale);
            _loop.pitch = _output.PitchAt(_pitch, scale);
            if (_isPaused)
            {
                _isPaused = false;
                _loop.UnPause();
            }
        }

        public void Stop()
        {
            _fade = 0f;
            _isPaused = false;
            _loop.Stop();
        }

        private float NextAccent()
        {
            return Random.Range(_sound.AccentMinSeconds, _sound.AccentMaxSeconds);
        }
    }
}
