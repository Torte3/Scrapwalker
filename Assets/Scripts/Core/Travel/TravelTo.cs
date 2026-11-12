using Scrapwalker.Core.Root;

namespace Scrapwalker.Core.Travel
{
    public sealed class TravelTo : Command
    {
        public RuinDefinition Ruin { get; }
        public bool IsRepeating { get; }

        public TravelTo(RuinDefinition ruin) : this(ruin, false)
        {
        }

        private TravelTo(RuinDefinition ruin, bool isRepeating)
        {
            Ruin = ruin;
            IsRepeating = isRepeating;
        }

        public static TravelTo Repeating(RuinDefinition ruin)
        {
            return new TravelTo(ruin, true);
        }

        internal override bool IsAllowedBy(CommandRule rule, World world) => rule.AllowsTravel(world, this);

        internal override CommandResult Check(World world)
        {
            DestinationStatus status = DestinationStatus.Of(world, Ruin);
            if (!status.IsOffRuin)
            {
                return CommandResult.Fail(CommandError.CannotChooseDestination);
            }

            if (status.IsDecided)
            {
                return CommandResult.Fail(CommandError.AlreadyHeading);
            }

            if (IsRepeating && !status.CanRepeat)
            {
                return CommandResult.Fail(CommandError.NotCleared);
            }

            return world.Exploration.CheckOpen(Ruin);
        }

        protected override void Run(World world)
        {
            Journey journey = world.Journey;
            if (!journey.IsTraveling)
            {
                JourneySystem.Leave(world);
            }

            Ruin.Plan.OnChosen(world);
            journey.HeadFor(Ruin, IsRepeating);
            world.Emit(new DestinationChosen(Ruin));
        }
    }
}
