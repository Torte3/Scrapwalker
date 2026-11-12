using System;
using System.Text;

namespace Scrapwalker.Game.UI
{
    public static class TextMarks
    {
        public static string Expand(string source, int shown, Func<string, bool, string> iconOf, string hidden)
        {
            var text = new StringBuilder(source.Length + 32);
            bool isHidden = false;
            int close = -1;
            for (int i = 0; i < source.Length; i++)
            {
                if (i == shown && !isHidden)
                {
                    text.Append(hidden);
                    isHidden = true;
                }

                if (i == close)
                {
                    close = -1;
                    continue;
                }

                char c = source[i];
                if (c == '{' && close < 0)
                {
                    int end = source.IndexOf('}', i + 1);
                    string icon = end > i + 1 ? iconOf(source.Substring(i + 1, end - i - 1), isHidden) : null;
                    if (icon != null)
                    {
                        text.Append(icon);
                        close = end;
                        continue;
                    }
                }

                text.Append(c);
            }

            return text.ToString();
        }
    }
}
