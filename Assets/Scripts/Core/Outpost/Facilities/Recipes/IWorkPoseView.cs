namespace Scrapwalker.Core.Outpost.Facilities.Recipes
{
    public interface IWorkPoseView<out T>
    {
        T Making();

        T Firing();
    }
}
