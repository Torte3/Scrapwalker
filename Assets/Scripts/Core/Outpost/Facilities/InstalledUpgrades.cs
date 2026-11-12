using System.Collections.Generic;
using Scrapwalker.Core.Framework;
using Scrapwalker.Core.Outpost.Facilities.Recipes;
using Scrapwalker.Core.Outpost.Roles;

namespace Scrapwalker.Core.Outpost.Facilities
{
    public sealed class InstalledUpgrades
    {
        private readonly FacilityDefinition _definition;
        private readonly List<string> _ids = new List<string>();

        public InstalledUpgrades(FacilityDefinition definition)
        {
            _definition = definition;
        }

        public float Total(FacilityStat stat)
        {
            float total = 0f;
            foreach (RecipeResult upgrade in Results())
            {
                total += upgrade.Adds(stat);
            }

            return total;
        }

        public bool Has(string id)
        {
            return _ids.Contains(id);
        }

        internal List<string> CreateMemento()
        {
            return new List<string>(_ids);
        }

        internal void Restore(List<string> memento, LoadContext load)
        {
            _ids.Clear();
            foreach (string id in memento)
            {
                _ids.Add((_definition.FindRecipe(id) ?? throw load.UnknownRecipe(id, _definition)).ID);
            }
        }

        internal void Add(string id)
        {
            if (!_ids.Contains(id))
            {
                _ids.Add(id);
            }
        }

        private IEnumerable<RecipeResult> Results()
        {
            foreach (string id in _ids)
            {
                RecipeDefinition recipe = _definition.FindRecipe(id);
                if (recipe != null)
                {
                    yield return recipe.Result;
                }
            }
        }
    }
}
