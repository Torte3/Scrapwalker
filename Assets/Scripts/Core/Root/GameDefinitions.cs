using System.Collections.Generic;
using System.Linq;
using Scrapwalker.Core.Combat;
using Scrapwalker.Core.Framework;
using Scrapwalker.Core.Items;
using Scrapwalker.Core.Outpost;
using Scrapwalker.Core.Outpost.Facilities;
using Scrapwalker.Core.Outpost.Upgrades;
using Scrapwalker.Core.Research;
using Scrapwalker.Core.Travel;

namespace Scrapwalker.Core.Root
{
    public sealed class GameDefinitions
    {
        public GameConfiguration Configuration = new GameConfiguration();
        public List<MaterialDefinition> Materials = new List<MaterialDefinition>();
        public List<FacilityDefinition> Facilities = new List<FacilityDefinition>();
        public List<AttackStyleDefinition> AttackStyles = new List<AttackStyleDefinition>();
        public List<EnemyTypeDefinition> EnemyTypes = new List<EnemyTypeDefinition>();
        public List<EnemyDefinition> Enemies = new List<EnemyDefinition>();
        public List<RuinDefinition> Ruins = new List<RuinDefinition>();
        public List<ResearchDefinition> Research = new List<ResearchDefinition>();
        public List<ItemTableDefinition> Tables = new List<ItemTableDefinition>();
        public List<OutpostUpgradeDefinition> Upgrades = new List<OutpostUpgradeDefinition>();

        public T Named<T>(string key) where T : Definition
        {
            return TryNamed(key, out T found) ? found : throw new KeyNotFoundException(typeof(T).Name + " が無い: " + key);
        }

        public bool TryNamed<T>(string key, out T found) where T : Definition
        {
            found = Find<T>(definition => definition.Key == key);
            return found != null;
        }

        public T ByID<T>(string id) where T : Definition
        {
            return Find<T>(definition => definition.ID == id);
        }

        private T Find<T>(System.Func<T, bool> match) where T : Definition
        {
            return All.OfType<T>().FirstOrDefault(match);
        }

        public IEnumerable<Definition> All =>
            Materials.Cast<Definition>().Concat(Tables).Concat(Research).Concat(Facilities).Concat(AttackStyles).Concat(EnemyTypes).Concat(Enemies).Concat(Ruins).Concat(Upgrades);

        public void Link(params object[] others)
        {
            var byID = new Dictionary<string, Definition>();
            foreach (Definition definition in All)
            {
                byID[definition.ID] = definition;
            }

            void Connect(DefinitionReference reference)
            {
                if (!string.IsNullOrEmpty(reference.ID))
                {
                    reference.TryLink(byID.TryGetValue(reference.ID, out Definition definition) ? definition : null);
                }
            }

            DefinitionReferences.ForEach(this, Connect);
            foreach (object other in others)
            {
                DefinitionReferences.ForEach(other, Connect);
            }
        }

        public RecipeDefinition FindRecipe(string id)
        {
            return Facilities.Select(facility => facility.FindRecipe(id)).FirstOrDefault(recipe => recipe != null);
        }

        public RecipeDefinition RecipeFor(Definition goal)
        {
            return goal != null ? Facilities.SelectMany(facility => facility.Recipes).FirstOrDefault(r => r.Result.Completes(goal)) : null;
        }

        public IEnumerable<FacilityDefinition> FacilitiesUnlockedBy(ResearchDefinition research)
        {
            return Facilities.Where(facility => facility.UnlockedBy.Research == research);
        }

        public IEnumerable<(FacilityDefinition Facility, RecipeDefinition Recipe)> RecipesUnlockedBy(ResearchDefinition research)
        {
            return Facilities.SelectMany(facility => facility.Recipes.Where(recipe => recipe.IsUnlockedBy(research)).Select(recipe => (facility, recipe)));
        }
    }
}
