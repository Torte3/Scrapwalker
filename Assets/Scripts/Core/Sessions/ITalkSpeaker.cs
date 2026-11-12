using Scrapwalker.Core.Travel.Talk;

namespace Scrapwalker.Core.Sessions
{
    public interface ITalkSpeaker
    {
        bool ShowsTalk { get; }
        SpokenLine Line { get; }
        int ShownChars { get; }
        bool IsLineShown { get; }

        void Click();
    }
}
