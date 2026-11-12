using Scrapwalker.Core.Items;
using Scrapwalker.Core.Outpost;
using Scrapwalker.Core.Outpost.Facilities;
using Scrapwalker.Core.Outpost.Facilities.Commands;
using Scrapwalker.Core.Root;

namespace Scrapwalker.Core.Sessions.Tutorial.Goals
{
    public sealed class OrderMaterialsGoal : FacilityGoal
    {
        private readonly NamedAmount[] _materials;

        internal OrderMaterialsGoal(string facility, NamedAmount[] materials) : base(facility)
        {
            _materials = materials;
        }

        internal override bool AllowsTask(World world, AddWorkTask task)
        {
            RecipeDefinition recipe = RecipeOf(world, task);
            if (task.Repeat || recipe == null)
            {
                return false;
            }

            foreach (NamedAmount a in _materials)
            {
                MaterialDefinition material = a.Of(world);
                int yield = recipe.Result.Yield(material);
                if (yield > 0)
                {
                    int left = a.Count - Secured(world, material);
                    return left > 0 && task.Times <= TimesFor(left, yield);
                }
            }

            return false;
        }

        public override bool Reached(World world)
        {
            return FindShort(world) == null;
        }

        public override T Guide<T>(World world, ITutorialGuide<T> guide)
        {
            Shortfall shortfall = FindShort(world);
            if (shortfall == null)
            {
                return guide.Order(FacilityOf(world), null, 1, null);
            }

            return guide.Order(FacilityOf(world), shortfall.Recipe, shortfall.Times, shortfall.Material);
        }

        protected override void CheckIn(FacilityDefinition facility, GameDefinitions definitions, ITutorialGoalCheck check)
        {
            foreach (NamedAmount a in _materials)
            {
                if (!a.Material.TryIn(definitions, out MaterialDefinition material))
                {
                    check.MissingMaterial(a.Material.Key);
                }
                else if (facility.RecipeMaking(material) == null)
                {
                    check.MissingRecipe(facility, a.Material.Key);
                }
            }
        }

        private Shortfall FindShort(World world)
        {
            foreach (NamedAmount a in _materials)
            {
                MaterialDefinition material = a.Of(world);
                int left = a.Count - Secured(world, material);
                if (left > 0)
                {
                    return new Shortfall(material, left, FacilityOf(world).RecipeMaking(material));
                }
            }

            return null;
        }

        private int Secured(World world, MaterialDefinition material)
        {
            return world.Ledger.HeldMaterial(material) + InFacilities(world, material) + UsedByHeldProducts(world, material);
        }

        private int InFacilities(World world, MaterialDefinition material)
        {
            int n = 0;
            FacilityDefinition definition = FacilityOf(world);
            foreach (Facility f in world.Facilities)
            {
                if (f.Definition != definition)
                {
                    continue;
                }

                n += f.Work.Inputs.CountOf(material);
                if (f.Work.WorkingTask != null)
                {
                    n += MaterialAmount.CountIn(f.Work.WorkingTask.Recipe.Inputs, material);
                }

                foreach (WorkTask task in f.Work.Queue.Tasks)
                {
                    n += task.Recipe.Result.Yield(material);
                }
            }

            return n;
        }

        private int UsedByHeldProducts(World world, MaterialDefinition material)
        {
            int n = 0;
            foreach (NamedAmount a in _materials)
            {
                MaterialDefinition made = a.Of(world);
                RecipeDefinition recipe = made != material ? FacilityOf(world).RecipeMaking(made) : null;
                if (recipe != null)
                {
                    int batches = TimesFor(world.Ledger.HeldMaterial(made), System.Math.Max(1, recipe.Result.Yield(made)));
                    n += batches * MaterialAmount.CountIn(recipe.Inputs, material);
                }
            }

            return n;
        }

        private sealed class Shortfall
        {
            public Shortfall(MaterialDefinition material, int left, RecipeDefinition recipe)
            {
                Material = material;
                Left = left;
                Recipe = recipe;
            }

            public MaterialDefinition Material { get; }
            public int Left { get; }
            public RecipeDefinition Recipe { get; }

            public int Times
            {
                get
                {
                    int perTask = Recipe != null ? System.Math.Max(1, Recipe.Result.Yield(Material)) : 1;
                    return TimesFor(Left, perTask);
                }
            }
        }

        private static int TimesFor(int count, int perTask)
        {
            return (count + perTask - 1) / perTask;
        }
    }
}
