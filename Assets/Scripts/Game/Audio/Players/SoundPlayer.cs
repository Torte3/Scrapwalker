using System.Collections.Generic;
using Scrapwalker.Game.Audio.Sounds;
using UnityEngine;

namespace Scrapwalker.Game.Audio.Players
{
    public sealed class SoundPlayer
    {
        private const int MaxSources = 32;

        private readonly Transform _parent;
        private readonly SEOutput _output;
        private readonly List<AudioSource> _sources = new List<AudioSource>();
        private readonly List<AudioSource> _all = new List<AudioSource>();
        private readonly Dictionary<AudioSource, int> _startedAt = new Dictionary<AudioSource, int>();
        private readonly Dictionary<AudioSource, float> _pitches = new Dictionary<AudioSource, float>();
        private readonly HashSet<AudioSource> _paused = new HashSet<AudioSource>();
        private readonly Dictionary<int, AudioSource> _voices = new Dictionary<int, AudioSource>();
        private readonly Stack<AudioSource> _spareVoices = new Stack<AudioSource>();
        private readonly List<int> _silent = new List<int>();
        private int _count;
        private float _timeScale = 1f;

        public SoundPlayer(Transform parent, SEOutput output)
        {
            _parent = parent;
            _output = output;
        }

        private bool IsPaused => _timeScale <= 0f;

        public void Play(SoundCue cue)
        {
            if (cue.IsEmpty)
            {
                return;
            }

            Start(SourceFor(cue), cue);
        }

        public void PlayVoice(int owner, SoundCue cue)
        {
            if (cue.IsEmpty)
            {
                return;
            }

            if (!_voices.TryGetValue(owner, out AudioSource source))
            {
                source = _spareVoices.Count > 0 ? _spareVoices.Pop() : NewSource();
                _voices[owner] = source;
            }

            Start(source, cue);
        }

        public void SetTimeScale(float scale)
        {
            if (scale == _timeScale)
            {
                return;
            }

            _timeScale = scale;
            foreach (AudioSource source in _all)
            {
                if (IsPaused)
                {
                    Pause(source);
                    continue;
                }

                source.outputAudioMixerGroup = _output.GroupAt(_timeScale);
                source.pitch = _output.PitchAt(_pitches[source], _timeScale);
                if (_paused.Remove(source))
                {
                    source.UnPause();
                }
            }
        }

        public void StopVoicesOf(System.Predicate<int> isGone)
        {
            _silent.Clear();
            foreach (KeyValuePair<int, AudioSource> voice in _voices)
            {
                if (isGone(voice.Key) || !IsSounding(voice.Value))
                {
                    _silent.Add(voice.Key);
                }
            }

            foreach (int owner in _silent)
            {
                StopVoice(owner);
            }
        }

        public void StopVoice(int owner)
        {
            if (_voices.TryGetValue(owner, out AudioSource source))
            {
                Stop(source);
                _voices.Remove(owner);
                _spareVoices.Push(source);
            }
        }

        public void StopAll()
        {
            foreach (AudioSource source in _sources)
            {
                Stop(source);
            }

            _silent.Clear();
            _silent.AddRange(_voices.Keys);
            foreach (int owner in _silent)
            {
                StopVoice(owner);
            }
        }

        private void Start(AudioSource source, SoundCue cue)
        {
            _paused.Remove(source);
            source.clip = cue.Clip;
            source.volume = cue.Volume;
            _pitches[source] = 1f + Random.Range(-cue.PitchJitter, cue.PitchJitter);
            source.pitch = _output.PitchAt(_pitches[source], _timeScale);
            source.Play();
            _startedAt[source] = ++_count;
            if (IsPaused)
            {
                Pause(source);
            }
        }

        private void Pause(AudioSource source)
        {
            if (source.isPlaying)
            {
                source.Pause();
                _paused.Add(source);
            }
        }

        private void Stop(AudioSource source)
        {
            _paused.Remove(source);
            source.Stop();
        }

        private bool IsSounding(AudioSource source)
        {
            return source.isPlaying || _paused.Contains(source);
        }

        private AudioSource SourceFor(SoundCue cue)
        {
            if (cue.MaxVoices > 0 && CountPlaying(cue.Clip) >= cue.MaxVoices)
            {
                return Oldest(cue.Clip);
            }

            foreach (AudioSource source in _sources)
            {
                if (!IsSounding(source))
                {
                    return source;
                }
            }

            return _sources.Count < MaxSources ? AddSource() : Oldest(null);
        }

        private int CountPlaying(AudioClip clip)
        {
            int n = 0;
            foreach (AudioSource source in _sources)
            {
                if (IsSounding(source) && source.clip == clip)
                {
                    n++;
                }
            }

            return n;
        }

        private AudioSource Oldest(AudioClip clip)
        {
            AudioSource oldest = null;
            foreach (AudioSource source in _sources)
            {
                if (IsSounding(source) && (clip == null || source.clip == clip) && (oldest == null || _startedAt[source] < _startedAt[oldest]))
                {
                    oldest = source;
                }
            }

            return oldest;
        }

        private AudioSource AddSource()
        {
            AudioSource source = NewSource();
            _sources.Add(source);
            return source;
        }

        private AudioSource NewSource()
        {
            var source = _parent.gameObject.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.outputAudioMixerGroup = _output.GroupAt(_timeScale);
            _startedAt[source] = 0;
            _pitches[source] = 1f;
            _all.Add(source);
            return source;
        }
    }
}
