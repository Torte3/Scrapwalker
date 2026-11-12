using Scrapwalker.Core.Root;
using Scrapwalker.Core.Travel;

namespace Scrapwalker.Core.Sessions.Tutorial.Goals
{
    public sealed class ChooseRuinGoal : TutorialGoal
    {
        private readonly NamedDefinition<RuinDefinition> _ruin;

        internal ChooseRuinGoal(string ruin)
        {
            _ruin = new NamedDefinition<RuinDefinition>(ruin);
        }

        public RuinDefinition RuinOf(World world)
        {
            return _ruin.Of(world);
        }

        internal override bool AllowsTravel(World world, TravelTo travel)
        {
            return travel.Ruin == RuinOf(world);
        }

        public override bool Reached(World world)
        {
            return world.Journey.Destination == RuinOf(world);
        }

        public override T Guide<T>(World world, ITutorialGuide<T> guide)
        {
            return guide.ChooseRuin(RuinOf(world));
        }

        public override void Check(GameDefinitions definitions, ITutorialGoalCheck check)
        {
            if (!_ruin.TryIn(definitions, out _))
            {
                check.MissingRuin(_ruin.Key);
            }
        }
    }
}
