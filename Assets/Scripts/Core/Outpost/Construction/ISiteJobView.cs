namespace Scrapwalker.Core.Outpost.Construction
{
    public interface ISiteJobView<out T>
    {
        T Build();

        T Rebuild();

        T Reassemble();

        T Dismantle();

        T Repair();
    }
}
