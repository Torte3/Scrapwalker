using System.Collections.Generic;
using Scrapwalker.Game.Audio.Sounds;
using UnityEngine;
using UnityEngine.Audio;

namespace Scrapwalker.Game.Audio
{
    [CreateAssetMenu(fileName = "SoundTable", menuName = "Scrapwalker/Sound Table")]
    public sealed class SoundTable : ScriptableObject
    {
        [SerializeField] private AudioMixerGroup _bgmGroup;
        [SerializeField] private AudioMixerGroup _seGroup;
        [SerializeField] private KeyedList<VolumeChannel, string> _volumes;
        [SerializeField] private KeyedList<SEChannelKey, SEChannel> _channels;
        [SerializeField] private KeyedList<MusicTrack, AudioClip> _music;
        [SerializeField] private float _crossfadeSeconds;
        [SerializeField] private float _duckSeconds;
        [SerializeField] private KeyedList<UISound, SoundCue> _ui;
        [SerializeField] private KeyedList<SceneSound, SoundCue> _scene;
        [SerializeField] private KeyedList<WorkSound, WorkLoop> _work;
        [SerializeField] private float _voiceMinSeconds;

        public AudioMixerGroup BGMGroup => _bgmGroup;
        public AudioMixerGroup SEGroup => _seGroup;
        public KeyedList<SEChannelKey, SEChannel> Channels => _channels;
        public KeyedList<MusicTrack, AudioClip> Music => _music;
        public float CrossfadeSeconds => _crossfadeSeconds;
        public float DuckSeconds => _duckSeconds;
        public KeyedList<UISound, SoundCue> UI => _ui;
        public KeyedList<SceneSound, SoundCue> Scene => _scene;
        public KeyedList<WorkSound, WorkLoop> Work => _work;
        public float VoiceMinSeconds => _voiceMinSeconds;

        public string VolumeParameter(VolumeChannel channel)
        {
            return _volumes[channel];
        }

#if UNITY_EDITOR
        public List<string> Problems()
        {
            var problems = new List<string>();
            problems.AddRange(_volumes.Problems());
            problems.AddRange(_channels.Problems());
            problems.AddRange(_music.Problems());
            problems.AddRange(_ui.Problems());
            problems.AddRange(_scene.Problems());
            problems.AddRange(_work.Problems());
            return problems;
        }

        public List<string> MissingMixerParameters()
        {
            AudioMixer mixer = _bgmGroup.audioMixer;
            var parameters = new List<string>();
            foreach (KeyValuePair<VolumeChannel, string> volume in _volumes)
            {
                parameters.Add(volume.Value);
            }

            foreach (KeyValuePair<SEChannelKey, SEChannel> channel in _channels)
            {
                parameters.Add(channel.Value.Pitch.First);
                parameters.Add(channel.Value.Pitch.Second);
            }

            var missing = new List<string>();
            foreach (string parameter in parameters)
            {
                if (string.IsNullOrEmpty(parameter) || !mixer.GetFloat(parameter, out _))
                {
                    missing.Add(parameter);
                }
            }

            return missing;
        }
#endif
    }
}
