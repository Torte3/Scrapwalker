namespace Scrapwalker.Core.Outpost
{
    public interface IPlaceView<out T>
    {
        T Storage();

        T Facility(Facility facility);

        T Floor();

        T Crack();

        T Gone();
    }
}
