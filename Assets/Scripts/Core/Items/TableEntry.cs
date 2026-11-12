using System;
using Scrapwalker.Core.Framework;

namespace Scrapwalker.Core.Items
{
    [Serializable]
    public struct TableEntry
    {
        public DefinitionReference<MaterialDefinition> TypeReference;
        public int Weight;

        public MaterialDefinition Type
        {
            get => TypeReference?.Definition;
            set => TypeReference = new DefinitionReference<MaterialDefinition>(value);
        }
    }
}
