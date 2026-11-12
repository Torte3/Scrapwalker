using System.Linq;
using Scrapwalker.Core.Framework;
using Scrapwalker.Core.Items;
using Scrapwalker.Core.Root;

namespace Scrapwalker.Core.Outpost.Facilities
{
    internal static class WorkSystem
    {
        internal static void Tick(World world)
        {
            foreach (Facility facility in world.Facilities)
            {
                if (!facility.IsOperational)
                {
                    continue;
                }

                if (facility.Work.ShouldHold(world.InCombat))
                {
                    facility.Work.Hold();
                }

                if (!facility.IsAttended)
                {
                    continue;
                }

                WorkJob job = facility.Work.Job;
                if (job != null)
                {
                    Advance(world, facility, job);
                }
                else
                {
                    StartNext(world, facility);
                }
            }
        }

        internal static Quality OutputGrade(World world, Facility facility, Quality input)
        {
            GameConfiguration configuration = world.Configuration;
            Quality facilityQuality = facility.BuiltQuality ?? Quality.Plain;
            Quality grade = PassThrough(configuration, input, facilityQuality);
            if (grade > facilityQuality)
            {
                grade = facilityQuality;
            }

            if (GameRandom.Value() < configuration.OutputQualityDownChance)
            {
                grade = QualityRules.Clamp((int)grade - 1);
            }

            return grade;
        }

        internal static Quality PassThrough(GameConfiguration configuration, Quality input, Quality facilityQuality)
        {
            return QualityRules.Clamp((int)input - configuration.Row(facilityQuality).OutputDrop);
        }

        internal static TaskStatus? Block(World world, Facility facility, WorkTask task)
        {
            WorkJob started = task == facility.Work.WorkingTask ? facility.Work.Job : task.HeldJob;
            return started != null ? JobBlock(world, facility, started) : StartBlock(world, facility, task);
        }

        private static TaskStatus? JobBlock(World world, Facility facility, WorkJob job)
        {
            RecipeDefinition recipe = job.Task.Recipe;
            return recipe.CanRunNow(world.InCombat) ? recipe.Result.ProgressBlock(world, facility, job) : TaskStatus.NotInCombat;
        }

        private static TaskStatus? StartBlock(World world, Facility facility, WorkTask task)
        {
            RecipeDefinition recipe = task.Recipe;
            if (!recipe.CanRunNow(world.InCombat))
            {
                return TaskStatus.NotInCombat;
            }

            TaskStatus? block = recipe.Result.ReadyBlock(world, facility, task);
            if (block.HasValue)
            {
                return block;
            }

            if (!recipe.Source.IsReady(world, facility, task))
            {
                return recipe.Source.IsOnTheWay(world, facility, task) ? TaskStatus.HaulingMaterials : TaskStatus.Materials;
            }

            if (world.Energy.Stored < recipe.Energy)
            {
                return TaskStatus.NotEnoughEnergy;
            }

            return recipe.Result.OutputBlock(world, facility);
        }

        private static void Advance(World world, Facility facility, WorkJob job)
        {
            RecipeDefinition recipe = job.Task.Recipe;
            if (recipe.CanRunNow(world.InCombat) && recipe.Result.Tick(world, facility, job))
            {
                Finish(world, facility, job);
            }
        }

        private static void StartNext(World world, Facility facility)
        {
            WorkTask task = facility.Work.Queue.First;
            if (task?.HeldJob != null)
            {
                if (task.Recipe.CanRunNow(world.InCombat))
                {
                    facility.Work.Resume(task);
                }

                return;
            }

            if (task == null || Block(world, facility, task).HasValue)
            {
                return;
            }

            RecipeDefinition recipe = task.Recipe;
            if (!recipe.Source.TryBegin(world, facility, task, world.Configuration.TicksAtLeastOne(recipe.Seconds)))
            {
                return;
            }

            world.Energy.Take(recipe.Energy);
            WorkJob job = facility.Work.Job;
            world.Log.Write(() => facility.LogName + " 着手 " + task.LogName + " " + recipe.ID + " 消費 " + string.Join(" ", job.Lots.Stacks.Select(stack => stack.LogText)) + " " + recipe.Seconds + "秒");
            if (recipe.Result.UsesUpInputs)
            {
                job.Lots.Clear();
            }

            recipe.Result.Begin(world, facility, job);
        }

        private static void Finish(World world, Facility facility, WorkJob job)
        {
            WorkTask task = job.Task;
            task.Recipe.Result.Finish(world, facility, job);
            facility.Work.Job = null;
            facility.Work.Queue.Finish(task);
            world.Log.Write(() => facility.LogName + " 終えた " + task.LogName + " " + task.Recipe.ID + (task.IsRepeating ? " →最後尾へ" : string.Empty));
        }
    }
}
