using System;
using Scrapwalker.Game.Look.Art;
using UnityEngine;

namespace Scrapwalker.Game.Definitions
{
    [Serializable]
    public sealed class FacilityLook
    {
        [SerializeField] private BuildCategory _category;
        [SerializeReference] private FacilityAct _act;
        [SerializeField] private bool _unlabeled;
        [SerializeReference] private StationLook _station;
        [SerializeReference] private WeaponLook _weapon;
        [SerializeReference] private FacilityGauge _gauge;
        [SerializeField] private ArtEntry _idle;
        [SerializeField] private ArtEntry _layers;
        [SerializeField] private ArtEntry _wake;
        [SerializeField] private ArtLayer _botLayer;
        [SerializeField] private ArtLayer _damage;

        public BuildCategory Category => _category;
        public FacilityAct Act => _act;
        public bool Unlabeled => _unlabeled;
        public StationLook Station => _station;
        public WeaponLook Weapon => _weapon;
        public FacilityGauge Gauge => _gauge ?? FacilityGauge.None;
        public ArtEntry Idle => _idle;
        public ArtEntry Layers => _layers;
        public ArtEntry Wake => _wake;
        public ArtLayer BotLayer => _botLayer;
        public ArtLayer Damage => _damage;
    }
}
