using System.Collections.Generic;

namespace Scrapwalker.Core.Travel.Talk
{
    public static class CuedTalkLists
    {
        public static CuedTalk TalkOn(this List<CuedTalk> talks, TalkCue cue)
        {
            return talks.Find(talk => talk.Cue == cue);
        }
    }
}
