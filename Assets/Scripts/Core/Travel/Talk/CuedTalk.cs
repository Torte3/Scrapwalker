using System;
using System.Collections.Generic;

namespace Scrapwalker.Core.Travel.Talk
{
    [Serializable]
    public sealed class CuedTalk
    {
        public TalkCue Cue;
        public float Delay;
        public List<TalkLine> Lines = new List<TalkLine>();
    }
}
