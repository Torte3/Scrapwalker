using Scrapwalker.Game.Audio.Players;
using Scrapwalker.Game.Audio.Sounds;

namespace Scrapwalker.Game.Audio
{
    public sealed class TalkVoice
    {
        private readonly SoundCue _voice;
        private readonly float _minSeconds;
        private readonly SoundPlayer _player;
        private string _text;
        private int _shown;
        private float _sinceVoice;

        public TalkVoice(SoundCue voice, float minSeconds, SoundPlayer player)
        {
            _voice = voice;
            _minSeconds = minSeconds;
            _player = player;
        }

        public void Tick(string text, int shown, float deltaTime)
        {
            _sinceVoice += deltaTime;
            if (!ReferenceEquals(text, _text) || shown < _shown)
            {
                _text = text;
                _shown = 0;
            }

            if (text == null || shown <= _shown)
            {
                return;
            }

            bool hasVoice = !char.IsWhiteSpace(text[shown - 1]);
            _shown = shown;
            if (hasVoice && _sinceVoice >= _minSeconds)
            {
                _player.Play(_voice);
                _sinceVoice = 0f;
            }
        }
    }
}
