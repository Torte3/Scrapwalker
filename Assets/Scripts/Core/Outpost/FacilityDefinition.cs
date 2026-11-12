using System;
using System.Collections.Generic;
using System.Linq;
using Scrapwalker.Core.Framework;
using Scrapwalker.Core.Items;
using Scrapwalker.Core.Outpost.Facilities;
using Scrapwalker.Core.Outpost.Facilities.Requirements;
using Scrapwalker.Core.Outpost.Grid;
using Scrapwalker.Core.Outpost.Roles;
using Scrapwalker.Core.Research;
using Scrapwalker.Core.Root;
using UnityEngine;

namespace Scrapwalker.Core.Outpost
{
    [Serializable]
    public sealed class FacilityDefinition : Definition
    {
        public int Width = 1;
        public int Height = 1;
        public List<MaterialAmount> Cost = new List<MaterialAmount>();
        public List<RecipeDefinition> Recipes = new List<RecipeDefinition>();
        public ResearchRequirement UnlockedBy = new ResearchRequirement();
        public bool IsIndestructible;
        public List<StatAmount> Stats = new List<StatAmount>();
        [SerializeReference] public List<FacilityRole> Roles = new List<FacilityRole>();

        public bool HasRecipes => Recipes.Count > 0;

        public RecipeDefinition RecipeMaking(MaterialDefinition material) => Recipes.Find(recipe => recipe.Makes(material));
        public bool HasRoutineWork => Recipes.Exists(r => !r.Result.WorksOnRoom);
        public bool HasStation => Recipes.Exists(r => r.NeedsStation);

        public bool HoldsMaterials => Roles.Exists(role => role.HoldsMaterials);
        public bool ConnectsFloors => Roles.Exists(role => role.ConnectsFloors);
        public bool StationsInCombat => Roles.Exists(role => role.StationsInCombat);
        public float HandOverSeconds => Roles.Sum(role => role.HandOverSeconds);
        public int? Limit => Roles.Select(role => role.BuildLimit).FirstOrDefault(limit => limit.HasValue);
        public Muzzle Muzzle => Roles.Select(role => role.Muzzle).FirstOrDefault(muzzle => muzzle != null);

        public int CostTotal => MaterialAmount.Total(Cost);

        public CellRect RectAt(Cell cell)
        {
            return new CellRect(cell, Width, Height);
        }

        public RecipeDefinition FindRecipe(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return null;
            }

            return Recipes.Find(recipe => recipe.ID == id);
        }

        public float BaseStat(FacilityStat stat)
        {
            return StatAmount.Total(Stats, stat);
        }

        public float StatBefore(FacilityStat stat, RecipeDefinition upgrade)
        {
            float total = BaseStat(stat);
            RecipeDefinition previous = upgrade.PreviousIn(this);
            for (int walked = 0; previous != null && walked < Recipes.Count; walked++)
            {
                total += previous.Result.Adds(stat);
                previous = previous.PreviousIn(this);
            }

            return total;
        }

        public CommandResult CheckAllowed(World world)
        {
            if (!IsListed(world))
            {
                return CommandResult.Fail(CommandError.Locked);
            }

            return LimitReached(world) ? CommandResult.Fail(CommandError.OverLimit, ID) : CommandResult.Success();
        }

        public bool IsListed(World world)
        {
            return UnlockedBy.IsMet(world, null);
        }

        public CommandResult CheckMaterials(World world, QualityFilter filter)
        {
            return world.Ledger.CanAfford(this, filter) ? CommandResult.Success() : CommandResult.Fail(CommandError.Shortage, shortage: world.Ledger.Shortage(this, filter));
        }

        public bool AllowsAnother(int standing)
        {
            return !Limit.HasValue || standing < Limit.Value;
        }

        private bool LimitReached(World world)
        {
            return !AllowsAnother(world.Facilities.CountIncludingUnbuilt(this));
        }

        internal void OnBuilt(Facility facility)
        {
            foreach (FacilityRole role in Roles)
            {
                role.OnBuilt(facility);
            }
        }
    }
}
