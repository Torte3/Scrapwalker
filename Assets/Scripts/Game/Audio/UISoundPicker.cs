using Scrapwalker.Game.Audio.Players;
using Scrapwalker.Game.Audio.Sounds;

namespace Scrapwalker.Game.Audio
{
    public sealed class UISoundPicker
    {
        private readonly KeyedList<UISound, SoundCue> _sounds;
        private readonly SoundPlayer _player;
        private UISound? _pending;

        public UISoundPicker(KeyedList<UISound, SoundCue> sounds, SoundPlayer player)
        {
            _sounds = sounds;
            _player = player;
        }

        public void Request(UISound sound)
        {
            if (!_pending.HasValue || sound > _pending.Value)
            {
                _pending = sound;
            }
        }

        public void Flush()
        {
            if (_pending.HasValue)
            {
                _player.Play(_sounds[_pending.Value]);
                _pending = null;
            }
        }
    }
}
