using System;
using Scrapwalker.Core.Framework;
using Scrapwalker.Core.Items;
using Scrapwalker.Core.Outpost.Floors;
using Scrapwalker.Core.Root;

namespace Scrapwalker.Core.Outpost.Facilities.Recipes.Results
{
    [Serializable]
    public sealed class MaterialResult : RecipeResult
    {
        public DefinitionReference<MaterialDefinition> MaterialReference =
            new DefinitionReference<MaterialDefinition>();
        public int Count = 1;

        public MaterialDefinition Material
        {
            get => MaterialReference.Definition;
            set => MaterialReference = new DefinitionReference<MaterialDefinition>(value);
        }

        public override int Yield(MaterialDefinition material)
        {
            return material == Material ? Count : 0;
        }

        public override void Check(FacilityDefinition facility, RecipeDefinition recipe, IRecipeCheck check)
        {
            if (Material == null)
            {
                check.MissingMaterial();
            }

            if (Count < 1)
            {
                check.BadRecipe();
            }
        }

        public override void Show(IRecipeResultView view)
        {
            view.Product(Material, Count);
        }

        internal override void Finish(World world, Facility facility, WorkJob job)
        {
            Quality grade = WorkSystem.OutputGrade(world, facility, job.InputQuality ?? Quality.Best);
            ItemStack items = ItemStack.Of(Material, grade, Count);
            SpillSystem.Drop(world, facility.Door, items);
            world.Emit(new MaterialsMade(facility, items));
        }
    }
}
