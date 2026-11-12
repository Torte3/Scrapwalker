using System.Collections.Generic;

namespace Scrapwalker.Core.Outpost.Hulls
{
    public sealed class HullMemento
    {
        public double CracksLost;
        public int NextCrackID;
        public List<CrackMemento> Cracks = new List<CrackMemento>();
    }
}
