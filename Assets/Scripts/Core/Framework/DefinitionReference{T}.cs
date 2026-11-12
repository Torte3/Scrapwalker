using System;

namespace Scrapwalker.Core.Framework
{
    [Serializable]
    public sealed class DefinitionReference<T> : DefinitionReference where T : Definition
    {
        [NonSerialized] private T _definition;

        public DefinitionReference()
        {
        }

        public DefinitionReference(T definition)
        {
            _definition = definition;
            ID = definition != null ? definition.ID : string.Empty;
        }

        public T Definition => _definition;

        public override Definition Target => _definition;

        internal override bool TryLink(Definition definition)
        {
            _definition = definition as T;
            return _definition != null;
        }
    }
}
