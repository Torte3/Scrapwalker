using System;
using System.Collections.Generic;
using System.Linq;
using Scrapwalker.Core.Combat.Waves;
using Scrapwalker.Core.Framework;
using Scrapwalker.Core.Items;
using Scrapwalker.Core.Outpost.Upgrades;
using Scrapwalker.Core.Travel.Talk;
using UnityEngine;

namespace Scrapwalker.Core.Travel
{
    [Serializable]
    public sealed class RuinDefinition : Definition
    {
        public List<DefinitionReference<OutpostUpgradeDefinition>> Requires = new List<DefinitionReference<OutpostUpgradeDefinition>>();
        [SerializeReference] public RuinVisitPlan Plan = new SalvagePlan();
        public List<WaveDefinition> Waves = new List<WaveDefinition>();
        public int[] QualityWeights = { 1, 0, 0, 0, 0 };
        public List<DefinitionReference<MaterialDefinition>> MainItems = new List<DefinitionReference<MaterialDefinition>>();
        public List<CuedTalk> Talks = new List<CuedTalk>();

        public int WaveCount => Waves.Count;

        public IEnumerable<OutpostUpgradeDefinition> NeededUpgrades => Requires.Select(required => required.Definition);

        public int UpgradesNeeded
        {
            get
            {
                var upgrades = new HashSet<OutpostUpgradeDefinition>();
                foreach (OutpostUpgradeDefinition upgrade in NeededUpgrades)
                {
                    upgrade.AddWithPrerequisites(upgrades);
                }

                return upgrades.Count;
            }
        }

        public bool Needs(OutpostUpgradeDefinition upgrade) => Requires.Exists(required => required.Definition == upgrade);

        public bool IsTakeoffWave(int number) => Plan.TakesOffAfter(this, number);

        public WaveDefinition WaveAt(int number)
        {
            return number >= 1 && number <= Waves.Count ? Waves[number - 1] : null;
        }

        public IEnumerable<WaveSpawn> SpawnOrder(int number)
        {
            WaveDefinition wave = WaveAt(number);
            return wave != null ? wave.Enemies.OrderBy(s => s.At) : Enumerable.Empty<WaveSpawn>();
        }

        public void AddFoundLines(List<SpokenLine> lines)
        {
            CuedTalk found = Talks.TalkOn(TalkCue.Found);
            if (found == null)
            {
                return;
            }

            foreach (TalkLine line in found.Lines)
            {
                lines.Add(new SpokenLine(line, this, 0));
            }
        }
    }
}
