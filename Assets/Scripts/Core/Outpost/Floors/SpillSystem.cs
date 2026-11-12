using System.Linq;
using Scrapwalker.Core.Items;
using Scrapwalker.Core.Outpost.Facilities;
using Scrapwalker.Core.Outpost.Grid;
using Scrapwalker.Core.Root;

namespace Scrapwalker.Core.Outpost.Floors
{
    internal static class SpillSystem
    {
        internal static void Drop(World world, GridPoint at, ItemStack items)
        {
            if (items.IsEmpty)
            {
                return;
            }

            world.Floor.Drop(at, items);
            world.Log.Write(() => "床に出た " + items.LogText + " @" + at);
        }

        internal static void Empty(World world, StackList stacks, GridPoint at)
        {
            foreach (ItemStack stack in stacks.TakeAll())
            {
                Drop(world, at, stack);
            }
        }

        internal static void SpillContents(World world, Facility f)
        {
            f.Site.Spill(world, f.Door);
            SpillInputs(world, f);
            SpillInventory(world, f);
        }

        internal static void SpillInventory(World world, Facility store)
        {
            if (store.Inventory == null)
            {
                return;
            }

            foreach (ItemStack stack in store.Inventory.TakeAll())
            {
                Drop(world, store.Door, stack);
            }
        }

        internal static void SpillWork(World world, Facility f)
        {
            if (f.Work.Job != null)
            {
                Empty(world, f.Work.Job.Lots, f.Door);
                f.Work.Job = null;
            }
        }

        internal static void SpillHeldJob(World world, Facility f, WorkTask task)
        {
            if (task.HeldJob != null)
            {
                Empty(world, task.HeldJob.Lots, f.Door);
                task.HeldJob = null;
            }
        }

        internal static void SpillInputs(World world, Facility f)
        {
            SpillWork(world, f);
            foreach (WorkTask task in f.Work.Queue.Tasks)
            {
                SpillHeldJob(world, f, task);
            }

            Empty(world, f.Work.Inputs, f.Door);
        }

        internal static void SpillUnneededInputs(World world, Facility f)
        {
            Empty(world, new StackList(f.Work.Inputs.RemoveAll(stack => !IsWanted(f, stack))), f.Door);
        }

        private static bool IsWanted(Facility f, ItemStack stack)
        {
            return f.Work.Queue.Tasks.Any(task => task.Filter.Accepts(stack.Quality) && task.Recipe.DeliveredInputs.Any(a => a.Type == stack.Type));
        }
    }
}
