using Scrapwalker.Core.Combat.Waves;
using Scrapwalker.Core.Travel;

namespace Scrapwalker.Game.MainView.Compose
{
    public sealed class ArmStages : IWavePhaseView<ArmStage>
    {
        private readonly RuinDefinition _ruin;

        public ArmStages(RuinDefinition ruin)
        {
            _ruin = ruin;
        }

        public ArmStage Idle() => ArmStage.Rest;

        public ArmStage Reaching() => ArmStage.Reaching;

        public ArmStage Fighting() => _ruin.Plan.IsDigging ? ArmStage.Digging : ArmStage.Rest;

        public ArmStage Haul() => ArmStage.Hauling;

        public ArmStage Resting() => ArmStage.Rest;
    }
}
