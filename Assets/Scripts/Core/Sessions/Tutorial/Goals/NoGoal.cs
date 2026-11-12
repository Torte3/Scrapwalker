using Scrapwalker.Core.Root;

namespace Scrapwalker.Core.Sessions.Tutorial.Goals
{
    public sealed class NoGoal : TutorialGoal
    {
        internal NoGoal()
        {
        }

        public override bool Reached(World world) => false;

        public override T Guide<T>(World world, ITutorialGuide<T> guide) => guide.Nothing();

        public override void Check(GameDefinitions definitions, ITutorialGoalCheck check)
        {
        }
    }
}
