using System;
using UnityEngine;

namespace Scrapwalker.Game.Look.Art.Sets
{
    [Serializable]
    public sealed class BotArt
    {
        [SerializeField] private BotPoseArt _walk = new BotPoseArt();
        [SerializeField] private BotPoseArt _climb = new BotPoseArt();
        [SerializeField] private BotPoseArt _weld = new BotPoseArt();
        [SerializeField] private ArtLayer _lowPowerEyes;

        public BotPoseArt Walk => _walk;
        public BotPoseArt Climb => _climb;
        public BotPoseArt Weld => _weld;
        public ArtLayer LowPowerEyes => _lowPowerEyes;
    }
}
