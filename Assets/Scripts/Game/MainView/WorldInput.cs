using Scrapwalker.Core.Outpost;
using Scrapwalker.Game.Look;
using UnityEngine;

namespace Scrapwalker.Game.MainView
{
    public struct WorldInput
    {
        public float GameDelta;
        public float TickShare;
        public float RealDelta;
        public bool HoldsTimeOfDay;
        public Facility Selected;
        public Facility Marked;
        public BuildGhost? Ghost;
        public float AIWake;
    }
}
