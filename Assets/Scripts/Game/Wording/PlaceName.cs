using Scrapwalker.Core.Outpost;
using Scrapwalker.Core.Root;
using Scrapwalker.Game.Definitions;
using Scrapwalker.Game.Wording.Words;

namespace Scrapwalker.Game.Wording
{
    public sealed class PlaceName : IPlaceView<string>
    {
        private readonly GameData _data;
        private readonly World _world;

        public PlaceName(GameData data, World world)
        {
            _data = data;
            _world = world;
        }

        public string Storage() => _data.StorageName;

        public string Facility(Facility facility) => _data.NameOf(facility.Definition);

        public string Floor() => SideWords.PlaceFloor;

        public string Crack() => SideWords.PlaceCrack;

        public string Gone() => string.Empty;
    }
}
