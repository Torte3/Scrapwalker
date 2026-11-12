using System;
using System.Collections.Generic;
using Scrapwalker.Core.Travel.Talk;

namespace Scrapwalker.Core.Sessions
{
    public sealed class TalkReader
    {
        private readonly Func<SpokenLine, string> _textOf;
        private IReadOnlyList<SpokenLine> _lines;
        private float _typed;

        public TalkReader(Func<SpokenLine, string> textOf)
        {
            _textOf = textOf;
        }

        public bool IsTalking => LineIndex >= 0;
        public int LineIndex { get; private set; } = -1;
        public SpokenLine Line => _lines[LineIndex];
        public int ShownChars => System.Math.Min(LineLength, (int)System.Math.Floor(_typed));
        public bool IsLineShown => ShownChars >= LineLength;
        private int LineLength => _textOf(Line).Length;

        public void Start(IReadOnlyList<SpokenLine> lines, int first = 0)
        {
            _lines = lines;
            LineIndex = lines != null && first < lines.Count ? first : -1;
            _typed = 0f;
        }

        public void Stop()
        {
            LineIndex = -1;
            _typed = 0f;
        }

        public void Type(float uiDeltaTime, float charsPerSecond)
        {
            if (IsTalking)
            {
                _typed += uiDeltaTime * charsPerSecond;
            }
        }

        public void Click()
        {
            if (!IsTalking)
            {
                return;
            }

            if (!IsLineShown)
            {
                _typed = LineLength;
                return;
            }

            _typed = 0f;
            LineIndex = LineIndex + 1 < _lines.Count ? LineIndex + 1 : -1;
        }
    }
}
