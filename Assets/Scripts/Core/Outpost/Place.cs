using Scrapwalker.Core.Outpost.Floors;
using Scrapwalker.Core.Outpost.Hulls;
using Scrapwalker.Core.Root;

namespace Scrapwalker.Core.Outpost
{
    public readonly struct Place : System.IEquatable<Place>
    {
        private readonly PlaceKind _kind;
        private readonly int _id;

        private Place(PlaceKind kind, int id)
        {
            _kind = kind;
            _id = id;
        }

        public static Place Storage => new Place(PlaceKind.Storage, 0);

        public static Place At(Facility f)
        {
            return new Place(PlaceKind.Facility, f.ID);
        }

        public static Place InStore(Facility store)
        {
            return new Place(PlaceKind.Store, store.ID);
        }

        public static Place AtCrack(Crack crack)
        {
            return new Place(PlaceKind.Crack, crack.ID);
        }

        public static Place AtPile(Pile pile)
        {
            return new Place(PlaceKind.Pile, pile.ID);
        }

        internal IPlace Resolve(World world)
        {
            switch (_kind)
            {
                case PlaceKind.Storage:
                    return StorageArea.Instance;
                case PlaceKind.Facility:
                    return world.Facilities.Find(_id);
                case PlaceKind.Store:
                    return world.Facilities.Find(_id)?.StoreRoom;
                case PlaceKind.Pile:
                    return world.Floor.Find(_id);
                case PlaceKind.Crack:
                    return world.Hull.FindCrack(_id);
                default:
                    throw new System.ArgumentOutOfRangeException(nameof(_kind), _kind, "番地の種類が無い");
            }
        }

        public T Show<T>(World world, IPlaceView<T> view)
        {
            IPlace place = Resolve(world);
            return place != null ? place.Show(view) : view.Gone();
        }

        internal string LogName(World world)
        {
            return Resolve(world)?.LogName ?? "?";
        }

        internal PlaceMemento CreateMemento()
        {
            return new PlaceMemento { Kind = _kind, ID = _id };
        }

        internal static Place Restore(PlaceMemento memento)
        {
            return new Place(memento.Kind, memento.ID);
        }

        public bool IsAt(Facility f)
        {
            return f != null && (_kind == PlaceKind.Facility || _kind == PlaceKind.Store) && _id == f.ID;
        }

        public bool Equals(Place other)
        {
            return _kind == other._kind && _id == other._id;
        }

        public override bool Equals(object value)
        {
            return value is Place other && Equals(other);
        }

        public override int GetHashCode()
        {
            return (int)_kind * 397 ^ _id;
        }

        public override string ToString()
        {
            return _kind + "#" + _id;
        }

        public static bool operator ==(Place a, Place b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(Place a, Place b)
        {
            return !a.Equals(b);
        }
    }
}
