using System;

namespace Scrapwalker.Core.Travel.Talk
{
    [Serializable]
    public struct TalkLine
    {
        public AIFace Face;
        public string Text;

        public TalkLine(AIFace face, string text)
        {
            Face = face;
            Text = text;
        }
    }
}
