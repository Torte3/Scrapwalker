using Scrapwalker.Core.Framework;

namespace Scrapwalker.Core.Sessions
{
    public sealed class Defeated : GameEvent
    {
    }

    public sealed class Restarted : GameEvent
    {
    }

    public sealed class DefeatBlackoutEnded : GameEvent
    {
        public DefeatBlackoutEnded(bool isFirstDefeat)
        {
            IsFirstDefeat = isFirstDefeat;
        }

        public bool IsFirstDefeat { get; }
    }
}
