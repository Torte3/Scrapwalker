using System;

namespace Scrapwalker.Core.Framework
{
    [Serializable]
    public abstract class Definition
    {
        public string ID = string.Empty;
        [NonSerialized] public string Key = string.Empty;

        public override string ToString()
        {
            return Key;
        }
    }
}
