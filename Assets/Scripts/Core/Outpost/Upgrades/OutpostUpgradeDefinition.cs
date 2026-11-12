using System;
using System.Collections.Generic;
using System.Linq;
using Scrapwalker.Core.Framework;
using Scrapwalker.Core.Travel.Talk;
using UnityEngine;

namespace Scrapwalker.Core.Outpost.Upgrades
{
    [Serializable]
    public sealed class OutpostUpgradeDefinition : Definition
    {
        public List<DefinitionReference<OutpostUpgradeDefinition>> Requires = new List<DefinitionReference<OutpostUpgradeDefinition>>();
        [SerializeReference] public List<UpgradeEffect> Effects = new List<UpgradeEffect>();
        public List<CuedTalk> Talks = new List<CuedTalk>();

        public IEnumerable<OutpostUpgradeDefinition> Prerequisites => Requires.Select(required => required.Definition);

        public void AddWithPrerequisites(ISet<OutpostUpgradeDefinition> into)
        {
            if (!into.Add(this))
            {
                return;
            }

            foreach (OutpostUpgradeDefinition prerequisite in Prerequisites)
            {
                prerequisite?.AddWithPrerequisites(into);
            }
        }
    }
}
