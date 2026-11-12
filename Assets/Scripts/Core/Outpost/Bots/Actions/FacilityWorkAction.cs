using Scrapwalker.Core.Outpost.Grid;
using Scrapwalker.Core.Root;

namespace Scrapwalker.Core.Outpost.Bots.Actions
{
    public sealed class FacilityWorkAction : PostAction
    {
        private bool _hasStartedRun;

        public FacilityWorkAction(WorkPriority priority, Facility facility)
            : base(priority, Place.At(facility), facility.Door)
        {
            Facility = facility;
        }

        internal FacilityWorkAction(FacilityWorkMemento memento, Facility facility)
            : base(memento)
        {
            Facility = facility;
            _hasStartedRun = memento.HasStartedRun;
        }

        public Facility Facility { get; }

        internal override BotActionMemento CreateMemento()
        {
            return WrittenAssignment(new FacilityWorkMemento { HasStartedRun = _hasStartedRun });
        }

        internal override GridPoint SpotFor(World world, Bot bot)
        {
            return Facility.NearestFront(world.Paths, bot.Position);
        }

        public override T Show<T>(IBotActionView<T> view)
        {
            return view.FacilityWork(this);
        }

        internal override bool UsesBattery(World world, Bot bot)
        {
            return bot.IsMoving || (!Facility.IsRemoved && Facility.HasWorkUnderway(world));
        }

        internal override bool IsValid(World world)
        {
            return IsOn() && Facility.HasFeasibleTask(world) && !Facility.Work.CurrentTask.Recipe.IsUnmanned;
        }

        internal override bool CanContinue(World world)
        {
            if (!IsOn())
            {
                return false;
            }

            return _hasStartedRun ? Facility.HasWorkUnderway(world) : IsValid(world);
        }

        protected override void Work(World world, Bot bot)
        {
            Facility.IsStaffed = true;
            Facility.StaffSpeed = bot.Speed(world.Configuration);
            if (Facility.HasWorkUnderway(world))
            {
                _hasStartedRun = true;
            }
        }

        private bool IsOn()
        {
            return !Facility.IsRemoved && Facility.IsOperational;
        }
    }
}
