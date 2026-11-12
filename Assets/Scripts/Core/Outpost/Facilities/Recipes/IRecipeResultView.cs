using Scrapwalker.Core.Items;
using Scrapwalker.Core.Outpost.Facilities.Recipes.Results;

namespace Scrapwalker.Core.Outpost.Facilities.Recipes
{
    public interface IRecipeResultView
    {
        void Product(MaterialDefinition material, int count);

        void Generate(EnergyResult energy);
    }
}
