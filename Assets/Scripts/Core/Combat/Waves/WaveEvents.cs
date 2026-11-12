using Scrapwalker.Core.Framework;
using Scrapwalker.Core.Travel;

namespace Scrapwalker.Core.Combat.Waves
{
    public sealed class WaveNearing : GameEvent
    {
        public WaveNearing(RuinDefinition ruin, int number)
        {
            Ruin = ruin;
            Number = number;
        }

        public RuinDefinition Ruin { get; }
        public int Number { get; }
    }

    public sealed class WaveStarted : GameEvent
    {
        public WaveStarted(RuinDefinition ruin, int number)
        {
            Ruin = ruin;
            Number = number;
        }

        public RuinDefinition Ruin { get; }
        public int Number { get; }
        public int Count => Ruin.WaveCount;
        public bool IsTakeoffWave => Ruin.IsTakeoffWave(Number);
    }

    public sealed class WaveCleared : GameEvent
    {
        public WaveCleared(RuinDefinition ruin, int number, bool leavesRuin)
        {
            Ruin = ruin;
            Number = number;
            LeavesRuin = leavesRuin;
        }

        public RuinDefinition Ruin { get; }
        public int Number { get; }
        public bool LeavesRuin { get; }
        public int Count => Ruin.WaveCount;
        public bool IsTakeoffWave => Ruin.IsTakeoffWave(Number);
    }
}
