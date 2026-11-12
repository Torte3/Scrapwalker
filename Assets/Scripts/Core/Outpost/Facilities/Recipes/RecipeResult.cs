using System;
using Scrapwalker.Core.Framework;
using Scrapwalker.Core.Items;
using Scrapwalker.Core.Outpost.Bots;
using Scrapwalker.Core.Outpost.Roles;
using Scrapwalker.Core.Root;

namespace Scrapwalker.Core.Outpost.Facilities.Recipes
{
    [Serializable]
    public abstract class RecipeResult
    {
        public virtual WorkPriority Priority => WorkPriority.Production;

        public virtual T ShowShelf<T>(IRecipeShelfView<T> view)
        {
            return view.Task();
        }

        public virtual T ShowPose<T>(IWorkPoseView<T> view)
        {
            return view.Making();
        }

        public virtual bool WorksOnRoom => false;

        public virtual int BotsMade => 0;

        internal virtual bool UsesUpInputs => false;

        internal virtual float Adds(FacilityStat stat)
        {
            return 0f;
        }

        public virtual bool Completes(Definition goal)
        {
            return false;
        }

        public virtual int Yield(MaterialDefinition material)
        {
            return 0;
        }

        public virtual bool IsSpent(World world, Facility facility, RecipeDefinition recipe)
        {
            return false;
        }

        public virtual void Check(FacilityDefinition facility, RecipeDefinition recipe, IRecipeCheck check)
        {
        }

        public virtual void Show(IRecipeResultView view)
        {
        }

        public virtual void Describe(World world, Facility facility, RecipeDefinition recipe, IRecipeEffects effects)
        {
        }

        internal virtual bool DescribeChange(World world, Facility facility, RecipeDefinition recipe, FacilityStat stat, float from, float to, IRecipeEffects effects)
        {
            return false;
        }

        internal virtual CommandResult CheckOffer(World world, Facility facility, RecipeDefinition recipe, int times)
        {
            return CommandResult.Success();
        }

        internal virtual TaskStatus? ReadyBlock(World world, Facility facility, WorkTask task)
        {
            return null;
        }

        internal virtual TaskStatus? OutputBlock(World world, Facility facility)
        {
            return null;
        }

        internal virtual TaskStatus? ProgressBlock(World world, Facility facility, WorkJob job)
        {
            return null;
        }

        internal virtual void Begin(World world, Facility facility, WorkJob job)
        {
        }

        internal virtual bool Tick(World world, Facility facility, WorkJob job)
        {
            return job.Time.Advance(facility.StaffSpeed);
        }

        internal abstract void Finish(World world, Facility facility, WorkJob job);
    }
}
