using UnityEngine;
using UnityEngine.Audio;

namespace Scrapwalker.Game.Audio.Players
{
    public sealed class MusicPlayer
    {
        private readonly float _crossfadeSeconds;
        private readonly float _duckSeconds;
        private readonly AudioSource[] _decks = new AudioSource[2];
        private readonly float[] _levels = new float[2];
        private readonly AudioSource _jingle;
        private int _current;
        private float _duck = 1f;

        public MusicPlayer(float crossfadeSeconds, float duckSeconds, Transform parent, AudioMixerGroup group)
        {
            _crossfadeSeconds = crossfadeSeconds;
            _duckSeconds = duckSeconds;
            for (int i = 0; i < _decks.Length; i++)
            {
                _decks[i] = NewSource(parent, group);
                _decks[i].loop = true;
            }

            _jingle = NewSource(parent, group);
        }

        public AudioClip Playing => _decks[_current].clip;

        public void Play(AudioClip clip)
        {
            if (clip == Playing)
            {
                return;
            }

            _current = 1 - _current;
            AudioSource deck = _decks[_current];
            deck.clip = clip;
            _levels[_current] = 0f;
            if (clip != null)
            {
                deck.Play();
            }
        }

        public void PlayJingle(AudioClip clip)
        {
            if (clip == null)
            {
                return;
            }

            _jingle.clip = clip;
            _jingle.Play();
        }

        public void Tick(float deltaTime)
        {
            float fade = _crossfadeSeconds > 0f ? deltaTime / _crossfadeSeconds : 1f;
            float duck = _duckSeconds > 0f ? deltaTime / _duckSeconds : 1f;
            _duck = Mathf.MoveTowards(_duck, _jingle.isPlaying ? 0f : 1f, duck);
            for (int i = 0; i < _decks.Length; i++)
            {
                AudioSource deck = _decks[i];
                _levels[i] = Mathf.MoveTowards(_levels[i], i == _current && deck.clip != null ? 1f : 0f, fade);
                deck.volume = _levels[i] * _duck;
                if (i != _current && _levels[i] <= 0f && deck.isPlaying)
                {
                    deck.Stop();
                }
            }
        }

        private static AudioSource NewSource(Transform parent, AudioMixerGroup group)
        {
            var source = parent.gameObject.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.outputAudioMixerGroup = group;
            return source;
        }
    }
}
