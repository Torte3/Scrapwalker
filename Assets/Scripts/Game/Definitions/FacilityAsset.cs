using System.Collections.Generic;
using Scrapwalker.Core.Outpost;
using Scrapwalker.Core.Outpost.Facilities;
using Scrapwalker.Game.Wording;
using UnityEngine;

namespace Scrapwalker.Game.Definitions
{
    [CreateAssetMenu(fileName = "Facility", menuName = "Scrapwalker/Facility")]
    public sealed class FacilityAsset : DefinitionAsset<FacilityDefinition>
    {
        [SerializeField] private string _name;
        [SerializeField] private List<RecipeWording> _recipes = new List<RecipeWording>();
        [SerializeField] private FacilityLook _look = new FacilityLook();

        public override string Name => _name;
        public IReadOnlyList<RecipeWording> Recipes => _recipes;
        public FacilityLook Look => _look;

#if UNITY_EDITOR
        private void OnValidate()
        {
            var seen = new HashSet<string>();
            bool changed = false;
            foreach (RecipeDefinition recipe in Value.Recipes)
            {
                if (string.IsNullOrEmpty(recipe.ID) || !seen.Add(recipe.ID))
                {
                    recipe.ID = System.Guid.NewGuid().ToString();
                    seen.Add(recipe.ID);
                    changed = true;
                }
            }

            if (changed)
            {
                UnityEditor.EditorUtility.SetDirty(this);
            }
        }
#endif
    }
}
