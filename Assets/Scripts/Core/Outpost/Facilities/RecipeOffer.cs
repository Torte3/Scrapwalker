using Scrapwalker.Core.Root;

namespace Scrapwalker.Core.Outpost.Facilities
{
    public readonly struct RecipeOffer
    {
        private RecipeOffer(bool isListed, CommandResult result)
        {
            IsListed = isListed;
            Result = result;
        }

        public bool IsListed { get; }
        public CommandResult Result { get; }

        internal static RecipeOffer Hidden(CommandResult result)
        {
            return new RecipeOffer(false, result);
        }

        internal static RecipeOffer Listed(CommandResult result)
        {
            return new RecipeOffer(true, result);
        }
    }
}
