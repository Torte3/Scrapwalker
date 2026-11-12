using System;
using Scrapwalker.Core.Outpost.Facilities;
using Scrapwalker.Core.Outpost.Facilities.Recipes;

namespace Scrapwalker.Core.Outpost.Roles
{
    [Serializable]
    public sealed class WeaponRole : FacilityRole
    {
        public Muzzle Barrel = new Muzzle();
        public string AutoRepeat = string.Empty;

        public override Muzzle Muzzle => Barrel;
        public override bool StationsInCombat => true;

        public override void Check(FacilityDefinition facility, IRecipeCheck check)
        {
            if (!string.IsNullOrEmpty(AutoRepeat) && facility.FindRecipe(AutoRepeat) == null)
            {
                check.MissingRecipe(AutoRepeat);
            }
        }

        internal override void OnBuilt(Facility facility)
        {
            RecipeDefinition auto = facility.Definition.FindRecipe(AutoRepeat);
            if (auto != null)
            {
                facility.Work.Queue.Add(auto, repeat: true);
            }
        }
    }
}
