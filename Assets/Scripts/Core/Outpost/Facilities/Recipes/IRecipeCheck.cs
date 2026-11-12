namespace Scrapwalker.Core.Outpost.Facilities.Recipes
{
    public interface IRecipeCheck
    {
        void MissingMaterial();

        void MissingResearch();

        void MissingRecipe(string id);

        void BadRecipe();

        void BadWeapon();
    }
}
