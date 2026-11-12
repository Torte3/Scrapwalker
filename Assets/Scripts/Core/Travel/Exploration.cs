using System.Collections.Generic;
using System.Linq;
using Scrapwalker.Core.Framework;
using Scrapwalker.Core.Outpost.Upgrades;
using Scrapwalker.Core.Root;

namespace Scrapwalker.Core.Travel
{
    public sealed class Exploration
    {
        private readonly OutpostUpgrades _upgrades;
        private readonly HashSet<RuinDefinition> _clearedRuins = new HashSet<RuinDefinition>();

        internal Exploration(OutpostUpgrades upgrades)
        {
            _upgrades = upgrades;
        }

        public RuinDefinition FurthestRuin { get; internal set; }

        internal void NoteReached(RuinDefinition ruin)
        {
            FurthestRuin = Further(FurthestRuin, ruin);
        }

        internal static RuinDefinition Further(RuinDefinition furthest, RuinDefinition reached)
        {
            return furthest == null || reached.UpgradesNeeded >= furthest.UpgradesNeeded ? reached : furthest;
        }

        public bool HasCleared(RuinDefinition ruin)
        {
            return _clearedRuins.Contains(ruin);
        }

        public bool IsOpen(RuinDefinition ruin)
        {
            return ruin.NeededUpgrades.All(_upgrades.IsDone);
        }

        public CommandResult CheckOpen(RuinDefinition ruin)
        {
            if (ruin == null)
            {
                return CommandResult.Fail(CommandError.NoSuchRuin);
            }

            return IsOpen(ruin) ? CommandResult.Success() : CommandResult.Fail(CommandError.Unreachable);
        }

        public IEnumerable<RuinDefinition> OpenedBy(IEnumerable<RuinDefinition> ruins, OutpostUpgradeDefinition upgrade)
        {
            return ruins.Where(ruin => ruin.Needs(upgrade) && IsOpen(ruin));
        }

        public bool HasClearedAllOpen(IEnumerable<RuinDefinition> ruins)
        {
            return ruins.Where(IsOpen).All(HasCleared);
        }

        public IReadOnlyList<OutpostUpgradeDefinition> UpgradesAwaited(IEnumerable<RuinDefinition> ruins)
        {
            var awaited = new List<OutpostUpgradeDefinition>();
            foreach (OutpostUpgradeDefinition needed in ruins.SelectMany(ruin => ruin.NeededUpgrades))
            {
                AddNextToward(needed, awaited);
            }

            return awaited;
        }

        private void AddNextToward(OutpostUpgradeDefinition upgrade, List<OutpostUpgradeDefinition> awaited)
        {
            if (_upgrades.IsDone(upgrade))
            {
                return;
            }

            if (!_upgrades.ArePrerequisitesDone(upgrade))
            {
                foreach (OutpostUpgradeDefinition prerequisite in upgrade.Prerequisites)
                {
                    AddNextToward(prerequisite, awaited);
                }
            }
            else if (!awaited.Contains(upgrade))
            {
                awaited.Add(upgrade);
            }
        }

        internal ExplorationMemento CreateMemento()
        {
            return new ExplorationMemento
            {
                Cleared = _clearedRuins.Select(ruin => ruin.ID).ToList(),
                Furthest = FurthestRuin?.ID,
            };
        }

        internal void Restore(ExplorationMemento memento, LoadContext load)
        {
            _clearedRuins.Clear();
            foreach (string id in memento.Cleared)
            {
                _clearedRuins.Add(load.Find<RuinDefinition>(id));
            }

            FurthestRuin = load.FindOrNone<RuinDefinition>(memento.Furthest);
        }

        internal void MarkCleared(RuinDefinition ruin)
        {
            _clearedRuins.Add(ruin);
        }
    }
}
