using Scrapwalker.Core.Root;
using Scrapwalker.Core.Sessions;

namespace Scrapwalker.Game.Audio
{
    public struct AudioFrame
    {
        public float UIDelta;
        public float GameDelta;
        public bool IsTitle;
        public World World;
        public float WakeProgress;
        public ITalkSpeaker Speaker;
        public SceneCues Scene;
    }
}
