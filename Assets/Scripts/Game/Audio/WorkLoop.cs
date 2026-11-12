using System;
using Scrapwalker.Game.Audio.Sounds;
using UnityEngine;

namespace Scrapwalker.Game.Audio
{
    [Serializable]
    public sealed class WorkLoop
    {
        [SerializeField] private SEChannelKey _channel;
        [SerializeField] private RepeatingSound _sound;

        public SEChannelKey Channel => _channel;
        public RepeatingSound Sound => _sound;
    }
}
