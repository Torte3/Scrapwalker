using System.Collections.Generic;
using Scrapwalker.Core.Items;

namespace Scrapwalker.Core.Root
{
    public sealed class ProgressMemento
    {
        public List<Quality> Obtained = new List<Quality>();
        public bool Escaped;
        public bool Defeated;
        public int EnemiesKilled;
        public int MaterialsCollected;
        public int WavesHeld;
    }
}
