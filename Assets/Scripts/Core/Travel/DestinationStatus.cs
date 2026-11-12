using Scrapwalker.Core.Root;

namespace Scrapwalker.Core.Travel
{
    public readonly struct DestinationStatus
    {
        public bool IsOffRuin { get; }
        public bool IsDecided { get; }
        public bool IsHeading { get; }
        public bool IsReachable { get; }
        public bool IsCleared { get; }
        public bool CanChoose => IsOffRuin && !IsDecided && IsReachable;
        public bool CanRepeat => CanChoose && IsCleared;

        private DestinationStatus(bool isOffRuin, bool isDecided, bool isHeading, bool isReachable, bool isCleared)
        {
            IsOffRuin = isOffRuin;
            IsDecided = isDecided;
            IsHeading = isHeading;
            IsReachable = isReachable;
            IsCleared = isCleared;
        }

        public static DestinationStatus Of(World world, RuinDefinition ruin)
        {
            Journey journey = world.Journey;
            bool isOffRuin = !journey.IsAtRuin;
            bool isHeading = ruin != null && journey.Destination == ruin;
            bool isReachable = ruin != null && world.Exploration.IsOpen(ruin);
            bool isCleared = ruin != null && world.Exploration.HasCleared(ruin);
            return new DestinationStatus(isOffRuin, journey.Destination != null, isHeading, isReachable, isCleared);
        }
    }
}
