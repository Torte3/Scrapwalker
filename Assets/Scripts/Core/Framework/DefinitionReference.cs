using System;

namespace Scrapwalker.Core.Framework
{
    [Serializable]
    public abstract class DefinitionReference
    {
        public string ID = string.Empty;

        public abstract Definition Target { get; }

        internal abstract bool TryLink(Definition definition);
    }
}
