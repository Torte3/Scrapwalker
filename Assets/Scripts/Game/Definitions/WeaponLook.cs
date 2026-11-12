using System;
using Scrapwalker.Game.Audio.Sounds;
using Scrapwalker.Game.Look.Art;
using UnityEngine;

namespace Scrapwalker.Game.Definitions
{
    [Serializable]
    public sealed class WeaponLook
    {
        [SerializeField] private float _recoilFrames;
        [SerializeField] private ShotLook _shot;
        [SerializeField] private ArtEntry _fire;
        [SerializeField] private WeaponSounds _sounds;

        public float RecoilFrames => _recoilFrames;
        public ShotLook Shot => _shot;
        public ArtEntry Fire => _fire;
        public WeaponSounds Sounds => _sounds;
    }
}
