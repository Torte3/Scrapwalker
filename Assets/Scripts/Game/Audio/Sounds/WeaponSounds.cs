using System;
using UnityEngine;

namespace Scrapwalker.Game.Audio.Sounds
{
    [Serializable]
    public sealed class WeaponSounds
    {
        [SerializeField] private SoundCue _fire = new SoundCue();
        [SerializeField] private RepeatingSound _firing = new RepeatingSound();
        [SerializeField] private float _firingHoldSeconds;
        [SerializeField] private SoundCue _land = new SoundCue();

        public SoundCue Fire => _fire;
        public RepeatingSound Firing => _firing;
        public float FiringHoldSeconds => _firingHoldSeconds;
        public bool FiresAsLoop => !_firing.Loop.IsEmpty;
        public SoundCue Land => _land;
    }
}
