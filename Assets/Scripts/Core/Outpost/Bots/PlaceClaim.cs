namespace Scrapwalker.Core.Outpost.Bots
{
    public readonly struct PlaceClaim
    {
        public PlaceClaim(Place place, PlaceUse use)
        {
            Place = place;
            Use = use;
        }

        public Place Place { get; }
        public PlaceUse Use { get; }
    }
}
