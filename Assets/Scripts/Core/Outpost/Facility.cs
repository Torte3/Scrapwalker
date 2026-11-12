using System.Collections.Generic;
using System.Linq;
using Scrapwalker.Core.Framework;
using Scrapwalker.Core.Items;
using Scrapwalker.Core.Outpost.Bots;
using Scrapwalker.Core.Outpost.Bots.Haul;
using Scrapwalker.Core.Outpost.Construction;
using Scrapwalker.Core.Outpost.Facilities;
using Scrapwalker.Core.Outpost.Facilities.Storage;
using Scrapwalker.Core.Outpost.Facilities.Recipes;
using Scrapwalker.Core.Outpost.Grid;
using Scrapwalker.Core.Outpost.Roles;
using Scrapwalker.Core.Root;

namespace Scrapwalker.Core.Outpost
{
    public sealed class Facility : IPlace, IWorksite
    {
        private const float FrontMargin = 0.5f;
        private const float RoundingTolerance = 0.0001f;
        private static readonly FiringQuestion FiringPose = new FiringQuestion();

        private readonly GameConfiguration _configuration;
        private float _regenerated;

        public Facility(int id, FacilityDefinition definition, Cell cell, GameConfiguration configuration)
        {
            ID = id;
            Definition = definition;
            Cell = cell;
            _configuration = configuration;
            HP = configuration.FacilityHP;
            Stationing = new Stationing(false, definition.StationsInCombat);
            Work = new Workshop(definition);
            Inventory = definition.HoldsMaterials ? new Inventory(() => (int)Stat(FacilityStat.StorageCapacity)) : null;
            Site = new Worksite(definition.Cost);
            StoreRoom = Inventory != null ? new StoreRoom(this) : null;
        }

        internal static Facility Restore(FacilityMemento memento, GameConfiguration configuration, LoadContext load)
        {
            var f = new Facility(memento.ID, load.Find<FacilityDefinition>(memento.Facility), Grid.Cell.Restore(memento.Cell), configuration);
            f.Condition = memento.Condition.Restore();
            f.HP = memento.HP;
            f.BuiltQuality = memento.Quality;
            f.IsStaffed = memento.Staffed;
            f.StaffSpeed = memento.StaffSpeed;
            f.Stationing = Stationing.Restore(memento.Stationing);
            f.Site.Restore(memento.Site, load);
            f.Work.Restore(memento.Work, f.Definition, load);
            if ((memento.Inventory != null) != (f.Inventory != null))
            {
                throw load.Malformed();
            }

            f.Inventory?.Restore(memento.Inventory, load);
            return f;
        }

        internal FacilityMemento CreateMemento()
        {
            return new FacilityMemento
            {
                ID = ID,
                Facility = Definition.ID,
                Cell = Cell.CreateMemento(),
                Condition = Condition.CreateMemento(),
                HP = HP,
                Quality = BuiltQuality,
                Staffed = IsStaffed,
                StaffSpeed = StaffSpeed,
                Stationing = Stationing.CreateMemento(),
                Site = Site.CreateMemento(),
                Work = Work.CreateMemento(),
                Inventory = Inventory?.CreateMemento(),
            };
        }

        public int ID { get; }
        public FacilityDefinition Definition { get; }
        public Cell Cell { get; internal set; }
        public FacilityCondition Condition { get; internal set; } = FacilityCondition.Planned;

        public Worksite Site { get; }
        public Workshop Work { get; }
        public Inventory Inventory { get; }
        internal StoreRoom StoreRoom { get; }
        internal bool IsRemoved { get; set; }

        public Quality? BuiltQuality { get; internal set; }
        public int HP { get; internal set; }

        public bool IsStaffed { get; internal set; }
        public bool IsAttended => IsStaffed || Work.CurrentTask?.Recipe.IsUnmanned == true;
        public float StaffSpeed { get; internal set; } = 1f;
        public Stationing Stationing { get; internal set; }

        public bool IsOperational => Condition.IsOperational;
        public bool IsStorageAccessible => Inventory != null && (IsOperational || Condition.IsBeingDismantled);
        public bool CanStoreMore => IsOperational && Inventory != null && Inventory.FreeSpace > 0;
        public int MaxHP => _configuration.FacilityHP;
        public int Power => (int)Stat(FacilityStat.Power);
        public CellRect Rect => Definition.RectAt(Cell);
        public GridPoint Door => Front(0.5f);

        public float SupplyShare => Condition.TakesSupplies ? Site.SupplyShare : 0f;

        public CellRect? RelocateRect
        {
            get
            {
                Cell? target = Condition.RelocateTarget;
                return target.HasValue ? Definition.RectAt(target.Value) : (CellRect?)null;
            }
        }

        internal string LogName => Definition.Key + "#" + ID;

        public bool IsStationed(bool inCombat)
        {
            return Definition.HasStation && IsOperational && Stationing.IsOn(inCombat);
        }

        public GridPoint Front(float share)
        {
            return new GridPoint(Cell.Column + FrontMargin + (Definition.Width - FrontMargin * 2f) * share, Cell.Row);
        }

        internal GridPoint NearestFront(Navigator paths, GridPoint from)
        {
            return paths.NearestBetween(from, Front(0f), Front(1f));
        }

        public TaskStatus StatusOf(World world, WorkTask task)
        {
            if (!IsOperational)
            {
                return task == Work.WorkingTask ? TaskStatus.Paused : Work.InputsReady(task, out _, out _) ? TaskStatus.Ready : TaskStatus.HaulingMaterials;
            }

            TaskStatus? block = WorkSystem.Block(world, this, task);
            if (block.HasValue)
            {
                return block.Value;
            }

            bool isCurrent = task == Work.CurrentTask;
            if (isCurrent && !IsAttended && !world.IsOccupied(Place.At(this)))
            {
                return TaskStatus.NoFreeBot;
            }

            if (task == Work.WorkingTask)
            {
                return IsAttended ? TaskStatus.Advancing : TaskStatus.Paused;
            }

            if (isCurrent && !IsAttended)
            {
                return TaskStatus.AwaitingBot;
            }

            return Work.InputsReady(task, out _, out _) ? TaskStatus.Ready : TaskStatus.HaulingMaterials;
        }

        public TaskStatus Status(World world)
        {
            WorkTask current = Work.CurrentTask;
            TaskStatus? task = current != null && IsOperational ? StatusOf(world, current) : (TaskStatus?)null;
            if (task.HasValue && task.Value.IsStop())
            {
                return task.Value;
            }

            if (Work.IsUpgrading)
            {
                return TaskStatus.Upgrading;
            }

            if (!Definition.HasRecipes || !IsOperational)
            {
                return TaskStatus.Operating;
            }

            if (!task.HasValue)
            {
                return Definition.HasRoutineWork ? TaskStatus.NoTasks : TaskStatus.Operating;
            }

            return task.Value;
        }

        public bool IsWorking(World world)
        {
            return Work.CurrentTask != null && StatusOf(world, Work.CurrentTask) == TaskStatus.Advancing;
        }

        internal bool HasWorkUnderway(World world)
        {
            return Work.Job != null && !WorkSystem.Block(world, this, Work.Job.Task).HasValue;
        }

        internal bool HasFeasibleTask(World world)
        {
            WorkTask current = Work.CurrentTask;
            return current != null && !WorkSystem.Block(world, this, current).HasValue;
        }

        public RecipeDefinition FiringRecipe => Work.CurrentTask != null && Fires(Work.CurrentTask.Recipe) ? Work.CurrentTask.Recipe : Definition.Recipes.Find(Fires);

        private static bool Fires(RecipeDefinition recipe)
        {
            return recipe.Result.ShowPose(FiringPose);
        }

        public float Stat(FacilityStat stat)
        {
            return Definition.BaseStat(stat) + Work.Upgrades.Total(stat);
        }

        public float GaugeFill(World world)
        {
            float fill = 0f;
            foreach (FacilityRole role in Definition.Roles)
            {
                fill = System.Math.Max(fill, role.Fill(world, this));
            }

            return fill;
        }

        public bool IsInUse(World world)
        {
            return Definition.Roles.Exists(role => role.IsInUse(world, this));
        }

        public bool IsRecovering(World world)
        {
            return IsOperational && Definition.Roles.Exists(role => role.IsRecovering(world));
        }

        public bool BuildAllowed(World world)
        {
            return Definition.AllowsAnother(world.Facilities.Count(other => other != this && other.Definition == Definition && other.Condition.IsStanding));
        }

        internal void TakeDamage(World world, int amount)
        {
            if (amount <= 0 || !Condition.CanBeDamaged || Definition.IsIndestructible)
            {
                return;
            }

            HP = HP > amount ? HP - amount : 0;
            world.Log.Write(() => LogName + " 被弾 -" + amount + " 体力 " + HP + "/" + MaxHP);
            if (HP == 0)
            {
                Condition.Break(world, this);
            }
        }

        internal void Regenerate()
        {
            if (!Condition.CanBeDamaged || HP >= MaxHP)
            {
                _regenerated = 0f;
                return;
            }

            _regenerated += _configuration.FacilityHPRegeneration * _configuration.TickSeconds;
            int whole = (int)(_regenerated + RoundingTolerance);
            if (whole <= 0)
            {
                return;
            }

            _regenerated -= whole;
            HP = System.Math.Min(MaxHP, HP + whole);
        }

        Place IWorksite.Place => Place.At(this);
        SiteJob IWorksite.Job => Condition.SiteJob;
        WorkPriority IWorksite.SitePriority => Condition.SitePriority;
        string IWorksite.LogName => LogName;
        bool IWorksite.IsGone => IsRemoved || !Condition.HasSiteWork;
        string IPlace.LogName => LogName;

        bool IWorksite.IsReady(World world)
        {
            return Condition.IsSiteReady(world, this);
        }

        GridPoint IWorksite.SpotFor(World world, Bot bot)
        {
            return NearestFront(world.Paths, bot.Position);
        }

        bool IWorksite.Advance(World world, Bot bot)
        {
            IsStaffed = true;
            return Site.Time.Advance(bot.Speed(world.Configuration));
        }

        void IWorksite.Complete(World world)
        {
            Condition.Complete(world, this);
        }

        T IPlace.Show<T>(IPlaceView<T> view)
        {
            return view.Facility(this);
        }

        DeliveryTarget IPlace.DeliveryFor(World world, GridPoint from, int count)
        {
            return Condition.IsUnderConstruction ? new SiteTarget(this) : new FacilityTarget(this);
        }

        public bool Covers(Cell cell)
        {
            return Rect.Covers(cell);
        }

        private sealed class FiringQuestion : IWorkPoseView<bool>
        {
            public bool Making() => false;

            public bool Firing() => true;
        }
    }
}
