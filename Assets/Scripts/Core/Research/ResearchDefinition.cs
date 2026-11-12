using System;
using System.Collections.Generic;
using Scrapwalker.Core.Framework;

namespace Scrapwalker.Core.Research
{
    [Serializable]
    public sealed class ResearchDefinition : Definition
    {
        public int Row;
        public List<DefinitionReference<ResearchDefinition>> Requires = new List<DefinitionReference<ResearchDefinition>>();

        public IEnumerable<ResearchDefinition> Prerequisites
        {
            get
            {
                foreach (DefinitionReference<ResearchDefinition> required in Requires)
                {
                    yield return required.Definition;
                }
            }
        }
    }
}
