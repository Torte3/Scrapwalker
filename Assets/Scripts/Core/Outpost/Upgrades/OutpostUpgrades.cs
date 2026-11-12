using System.Collections.Generic;
using System.Linq;
using Scrapwalker.Core.Framework;
using Scrapwalker.Core.Root;

namespace Scrapwalker.Core.Outpost.Upgrades
{
    public sealed class OutpostUpgrades
    {
        private readonly IReadOnlyList<UpgradeEffect> _start;
        private readonly HashSet<OutpostUpgradeDefinition> _done = new HashSet<OutpostUpgradeDefinition>();

        internal OutpostUpgrades(IReadOnlyList<UpgradeEffect> start)
        {
            _start = start;
            Traits = OutpostTraits.Of(_start, _done);
        }

        public OutpostTraits Traits { get; private set; }

        public bool IsDone(OutpostUpgradeDefinition upgrade)
        {
            return _done.Contains(upgrade);
        }

        public bool ArePrerequisitesDone(OutpostUpgradeDefinition upgrade)
        {
            return upgrade.Prerequisites.All(IsDone);
        }

        public OutpostTraits TraitsBefore(OutpostUpgradeDefinition upgrade)
        {
            var upgrades = new HashSet<OutpostUpgradeDefinition>(_done);
            foreach (OutpostUpgradeDefinition prerequisite in upgrade.Prerequisites)
            {
                prerequisite.AddWithPrerequisites(upgrades);
            }

            return OutpostTraits.Of(_start, upgrades);
        }

        internal void Add(OutpostUpgradeDefinition upgrade)
        {
            _done.Add(upgrade);
            Traits = OutpostTraits.Of(_start, _done);
        }

        public static TimeOfDay TimeOfDayOf(OutpostUpgradesMemento memento, GameDefinitions definitions)
        {
            return OutpostTraits.Of(definitions.Configuration.StartEffects, definitions.Upgrades.Where(upgrade => memento.Done.Contains(upgrade.ID))).TimeOfDay;
        }

        internal OutpostUpgradesMemento CreateMemento()
        {
            return new OutpostUpgradesMemento { Done = _done.Select(upgrade => upgrade.ID).ToList() };
        }

        internal void Restore(OutpostUpgradesMemento memento, LoadContext load)
        {
            _done.Clear();
            foreach (string id in memento.Done)
            {
                _done.Add(load.Find<OutpostUpgradeDefinition>(id));
            }

            Traits = OutpostTraits.Of(_start, _done);
        }
    }
}
