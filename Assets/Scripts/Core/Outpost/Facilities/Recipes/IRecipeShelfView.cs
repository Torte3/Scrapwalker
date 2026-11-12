namespace Scrapwalker.Core.Outpost.Facilities.Recipes
{
    public interface IRecipeShelfView<out T>
    {
        T Task();

        T Upgrade();

        T Research();
    }
}
