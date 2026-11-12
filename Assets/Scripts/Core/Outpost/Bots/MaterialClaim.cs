using Scrapwalker.Core.Items;

namespace Scrapwalker.Core.Outpost.Bots
{
    public readonly struct MaterialClaim
    {
        public MaterialClaim(Place place, MaterialFlow flow, MaterialDefinition type, int count, int? task)
        {
            Place = place;
            Flow = flow;
            Type = type;
            Count = count;
            Task = task;
        }

        public Place Place { get; }
        public MaterialFlow Flow { get; }
        public MaterialDefinition Type { get; }
        public int Count { get; }
        public int? Task { get; }
    }
}
