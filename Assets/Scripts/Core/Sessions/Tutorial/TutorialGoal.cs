using Scrapwalker.Core.Root;
using Scrapwalker.Core.Sessions.Tutorial.Goals;

namespace Scrapwalker.Core.Sessions.Tutorial
{
    public abstract class TutorialGoal : CommandRule
    {
        public static readonly TutorialGoal None = new NoGoal();

        public static TutorialGoal Place(string facility) => new PlaceFacilityGoal(facility);
        public static TutorialGoal Order(string facility, params NamedAmount[] materials) => new OrderMaterialsGoal(facility, materials);
        public static TutorialGoal Choose(string ruin) => new ChooseRuinGoal(ruin);
        public static TutorialGoal OrderOf(string facility, string recipe) => new OrderRecipeGoal(facility, recipe);

        internal override CommandError Refusal => CommandError.Refused;

        internal override bool AllowsOther(World world, Command command) => false;

        internal override bool AllowsLockOn(World world, Command lockOn) => true;

        internal override bool AllowsSaving => false;

        public abstract bool Reached(World world);

        public abstract T Guide<T>(World world, ITutorialGuide<T> guide);

        public abstract void Check(GameDefinitions definitions, ITutorialGoalCheck check);
    }
}
