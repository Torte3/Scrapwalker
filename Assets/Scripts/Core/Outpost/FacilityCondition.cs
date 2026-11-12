using System.Collections.Generic;
using System.Linq;
using Scrapwalker.Core.Outpost.Bots;
using Scrapwalker.Core.Outpost.Bots.Haul;
using Scrapwalker.Core.Outpost.Construction;
using Scrapwalker.Core.Outpost.Facilities;
using Scrapwalker.Core.Outpost.Floors;
using Scrapwalker.Core.Outpost.Grid;
using Scrapwalker.Core.Root;

namespace Scrapwalker.Core.Outpost
{
    public abstract class FacilityCondition
    {
        private static readonly IReadOnlyDictionary<FacilityOrder, CommandError> DefaultRefusals = new Dictionary<FacilityOrder, CommandError>
        {
            { FacilityOrder.AddTask, CommandError.NotBuilt },
            { FacilityOrder.Dismantle, CommandError.CancelPlanInstead },
            { FacilityOrder.Relocate, CommandError.None },
            { FacilityOrder.CancelPlacement, CommandError.NotPlanned },
            { FacilityOrder.CancelDismantle, CommandError.NotDismantling },
        };

        public static readonly FacilityCondition Planned = new PlannedCondition();
        public static readonly FacilityCondition Rebuilding = new RebuildingCondition();
        public static readonly FacilityCondition Reassembling = new ReassemblingCondition();
        public static readonly FacilityCondition Operational = new OperationalCondition();
        public static readonly FacilityCondition Dismantling = new DismantlingCondition();

        private readonly Dictionary<FacilityOrder, CommandError> _refusals;

        private protected FacilityCondition(FacilityStage stage, params (FacilityOrder Order, CommandError Error)[] refusals)
        {
            Stage = stage;
            _refusals = DefaultRefusals.ToDictionary(pair => pair.Key, pair => pair.Value);
            foreach ((FacilityOrder order, CommandError error) in refusals)
            {
                _refusals[order] = error;
            }
        }

        public static FacilityCondition RelocatingTo(Cell target)
        {
            return new RelocatingCondition(target);
        }

        internal static FacilityCondition Of(FacilityStage stage, Cell? target)
        {
            switch (stage)
            {
                case FacilityStage.Planned:
                    return Planned;
                case FacilityStage.Rebuilding:
                    return Rebuilding;
                case FacilityStage.Reassembling:
                    return Reassembling;
                case FacilityStage.Operational:
                    return Operational;
                case FacilityStage.Dismantling:
                    return Dismantling;
                case FacilityStage.Relocating:
                    return RelocatingTo(target ?? throw new System.ArgumentNullException(nameof(target)));
                default:
                    throw new System.ArgumentOutOfRangeException(nameof(stage), stage, "部屋の段階が無い");
            }
        }

        internal FacilityStage Stage { get; }
        public virtual bool IsOperational => false;
        public virtual bool IsUnderConstruction => false;
        public virtual bool IsBeingDismantled => false;
        public virtual bool IsRebuilding => false;
        public virtual bool IsReassembling => false;
        public virtual Cell? RelocateTarget => null;

        public bool IsStanding => IsOperational || IsBeingDismantled;
        public bool CanBeDamaged => IsStanding;
        public bool HasSiteWork => IsUnderConstruction || IsBeingDismantled;

        internal virtual WorkPriority SitePriority => WorkPriority.Works;
        internal virtual bool TakesSupplies => false;
        internal virtual SiteJob SiteJob => SiteJob.Build;

        public abstract T Show<T>(IFacilityConditionView<T> view);

        public CommandError Refusal(FacilityOrder order)
        {
            return _refusals[order];
        }

        internal ConditionMemento CreateMemento()
        {
            return new ConditionMemento { Stage = Stage, Target = RelocateTarget?.CreateMemento() };
        }

        internal virtual bool IsSiteReady(World world, Facility f)
        {
            return f.Site.IsSupplied && f.BuildAllowed(world);
        }

        internal static void Place(World world, Facility f)
        {
            f.Condition = Planned;
            f.Site.Time.Start(world.Configuration.TicksFor(world.Configuration.BuildSeconds));
        }

        internal static void Wreck(World world, Facility f)
        {
            SpillSystem.SpillInputs(world, f);
            SpillSystem.SpillInventory(world, f);
            f.HP = 0;
            f.IsStaffed = false;
            f.Condition = Rebuilding;
            f.Site.Time.Start(world.Configuration.TicksFor(world.Configuration.BuildSeconds));
        }

        internal static void Remove(World world, Facility f)
        {
            SpillSystem.SpillContents(world, f);
            world.RemoveFacility(f);
            world.Emit(new FacilityRemoved(f));
        }

        internal virtual void Relocate(World world, Facility f, Cell to)
        {
            throw new System.InvalidOperationException(Stage + " cannot be relocated");
        }

        internal virtual void Dismantle(World world, Facility f)
        {
            throw new System.InvalidOperationException(Stage + " cannot be dismantled");
        }

        internal virtual void CancelPlacement(World world, Facility f)
        {
            throw new System.InvalidOperationException(Stage + " is not planned");
        }

        internal virtual void CancelDismantle(World world, Facility f)
        {
            throw new System.InvalidOperationException(Stage + " is not being dismantled");
        }

        internal virtual void Complete(World world, Facility f)
        {
            throw new System.InvalidOperationException(Stage + " has no site work");
        }

        internal virtual void Break(World world, Facility f)
        {
            throw new System.InvalidOperationException(Stage + " cannot be damaged");
        }

        private static void BeginDismantle(World world, Facility f, FacilityCondition dismantling)
        {
            f.Condition = dismantling;
            f.Site.Time.Start(world.Configuration.TicksFor(world.Configuration.DismantleSeconds));
            SpillSystem.SpillInputs(world, f);
        }

        private static void CompleteConstruction(Facility f)
        {
            f.Site.Time.End();
            f.BuiltQuality = f.Site.Delivered.AverageQuality;
            f.Condition = Operational;
            f.HP = f.MaxHP;
        }

        private abstract class BuildingCondition : FacilityCondition
        {
            protected BuildingCondition(FacilityStage stage, params (FacilityOrder Order, CommandError Error)[] refusals)
                : base(stage, refusals.Prepend((FacilityOrder.CancelPlacement, CommandError.None)).ToArray())
            {
            }

            public override bool IsUnderConstruction => true;

            internal override void Relocate(World world, Facility f, Cell to)
            {
                SpillBeforeMove(world, f);
                f.Cell = to;
                f.Site.Time.Restart();
                HaulSystem.Reroute(world, f);
            }

            internal override void CancelPlacement(World world, Facility f)
            {
                Remove(world, f);
            }

            internal override void Complete(World world, Facility f)
            {
                CompleteConstruction(f);
            }

            protected virtual void SpillBeforeMove(World world, Facility f)
            {
                f.Site.Spill(world, f.Door);
            }
        }

        private sealed class PlannedCondition : BuildingCondition
        {
            public PlannedCondition()
                : base(FacilityStage.Planned)
            {
            }

            public override T Show<T>(IFacilityConditionView<T> view)
            {
                return view.Planned();
            }

            internal override bool TakesSupplies => true;

            internal override void Complete(World world, Facility f)
            {
                CompleteConstruction(f);
                f.Definition.OnBuilt(f);
            }
        }

        private sealed class RebuildingCondition : BuildingCondition
        {
            public RebuildingCondition()
                : base(
                    FacilityStage.Rebuilding,
                    (FacilityOrder.Relocate, CommandError.RepairFirst),
                    (FacilityOrder.CancelPlacement, CommandError.NotPlanned),
                    (FacilityOrder.Dismantle, CommandError.None))
            {
            }

            public override T Show<T>(IFacilityConditionView<T> view)
            {
                return view.Rebuilding();
            }

            public override bool IsRebuilding => true;
            internal override bool TakesSupplies => true;
            internal override WorkPriority SitePriority => WorkPriority.Rebuild;
            internal override SiteJob SiteJob => SiteJob.Rebuild;

            internal override void Dismantle(World world, Facility f)
            {
                Remove(world, f);
            }
        }

        private sealed class ReassemblingCondition : BuildingCondition
        {
            public ReassemblingCondition()
                : base(FacilityStage.Reassembling)
            {
            }

            public override T Show<T>(IFacilityConditionView<T> view)
            {
                return view.Reassembling();
            }

            public override bool IsReassembling => true;
            internal override SiteJob SiteJob => SiteJob.Reassemble;

            protected override void SpillBeforeMove(World world, Facility f)
            {
            }
        }

        private sealed class OperationalCondition : FacilityCondition
        {
            public OperationalCondition()
                : base(FacilityStage.Operational, (FacilityOrder.AddTask, CommandError.None), (FacilityOrder.Dismantle, CommandError.None))
            {
            }

            public override T Show<T>(IFacilityConditionView<T> view)
            {
                return view.Operational();
            }

            public override bool IsOperational => true;

            internal override void Dismantle(World world, Facility f)
            {
                BeginDismantle(world, f, Dismantling);
            }

            internal override void Relocate(World world, Facility f, Cell to)
            {
                BeginDismantle(world, f, RelocatingTo(to));
            }

            internal override void Break(World world, Facility f)
            {
                Wreck(world, f);
            }
        }

        private abstract class TakingDownCondition : FacilityCondition
        {
            protected TakingDownCondition(FacilityStage stage, CommandError relocate)
                : base(
                    stage,
                    (FacilityOrder.AddTask, CommandError.BeingDismantled),
                    (FacilityOrder.Dismantle, CommandError.AlreadyDismantling),
                    (FacilityOrder.Relocate, relocate),
                    (FacilityOrder.CancelDismantle, CommandError.None))
            {
            }

            public override bool IsBeingDismantled => true;
            internal override SiteJob SiteJob => SiteJob.Dismantle;

            internal override bool IsSiteReady(World world, Facility f)
            {
                return true;
            }

            internal override void CancelDismantle(World world, Facility f)
            {
                f.Condition = Operational;
                f.Site.Time.End();
                foreach (Facility over in world.Facilities.Where(other => other.Definition == f.Definition && other.Condition.IsUnderConstruction && !other.BuildAllowed(world)).ToList())
                {
                    Remove(world, over);
                }
            }

            internal override void Complete(World world, Facility f)
            {
                Remove(world, f);
            }

            internal override void Break(World world, Facility f)
            {
                Remove(world, f);
            }
        }

        private sealed class DismantlingCondition : TakingDownCondition
        {
            public DismantlingCondition()
                : base(FacilityStage.Dismantling, CommandError.DismantleFirst)
            {
            }

            public override T Show<T>(IFacilityConditionView<T> view)
            {
                return view.Dismantling();
            }
        }

        private sealed class RelocatingCondition : TakingDownCondition
        {
            private readonly Cell _target;

            public RelocatingCondition(Cell target)
                : base(FacilityStage.Relocating, CommandError.None)
            {
                _target = target;
            }

            public override T Show<T>(IFacilityConditionView<T> view)
            {
                return view.Relocating(_target);
            }

            public override Cell? RelocateTarget => _target;

            internal override void Relocate(World world, Facility f, Cell to)
            {
                f.Condition = RelocatingTo(to);
            }

            internal override void Complete(World world, Facility f)
            {
                SpillSystem.SpillInventory(world, f);
                f.Cell = _target;
                f.Condition = Reassembling;
                f.Site.Time.Start(world.Configuration.TicksFor(world.Configuration.RelocateSeconds));
            }
        }
    }
}
