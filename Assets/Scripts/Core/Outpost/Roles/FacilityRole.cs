using System;
using Scrapwalker.Core.Outpost.Facilities.Recipes;
using Scrapwalker.Core.Root;

namespace Scrapwalker.Core.Outpost.Roles
{
    [Serializable]
    public abstract class FacilityRole
    {
        public virtual bool HoldsMaterials => false;
        public virtual bool ConnectsFloors => false;
        public virtual float HandOverSeconds => 0f;
        public virtual int? BuildLimit => null;
        public virtual Muzzle Muzzle => null;
        public virtual bool StationsInCombat => false;

        public virtual float Fill(World world, Facility facility)
        {
            return 0f;
        }

        public virtual bool IsInUse(World world, Facility facility)
        {
            return false;
        }

        public virtual bool IsRecovering(World world)
        {
            return false;
        }

        public virtual void Check(FacilityDefinition facility, IRecipeCheck check)
        {
        }

        internal virtual void OnBuilt(Facility facility)
        {
        }
    }
}
