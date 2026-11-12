using Scrapwalker.Core.Combat.Waves;
using Scrapwalker.Game.Wording.Words;

namespace Scrapwalker.Game.UI.Screens.TopBar
{
    public sealed class WaveTitles : IWavePhaseView<string>
    {
        private readonly int _number;
        private readonly int _count;

        public WaveTitles(int number, int count)
        {
            _number = number;
            _count = count;
        }

        public string Idle() => TopBarWords.Wave(_number, _count);

        public string Reaching() => TopBarWords.Wave(_number, _count);

        public string Fighting() => TopBarWords.Wave(_number, _count);

        public string Haul() => TopBarWords.WaveHauling(_number, _count);

        public string Resting() => TopBarWords.Wave(_number, _count);
    }
}
