using System.Text;
using Scrapwalker.Core.Travel;
using Scrapwalker.Core.Travel.Talk;
using Scrapwalker.Game.Definitions;

namespace Scrapwalker.Game.Wording
{
    public static class TalkText
    {
        public const string ItemsMark = "{items}";
        public const string FoundMark = "{found}";

        public static string Wording(this SpokenLine line, GameData data)
        {
            string text = line.Text.Replace(FoundMark, line.Found.ToString());
            return line.About != null ? text.Replace(ItemsMark, MainItems(data, line.About)) : text;
        }

        private static string MainItems(GameData data, RuinDefinition ruin)
        {
            var items = new StringBuilder();
            for (int i = 0; i < ruin.MainItems.Count; i++)
            {
                if (i > 0)
                {
                    items.Append(i == 1 ? "や" : "、");
                }

                items.Append('{').Append(data.Asset(ruin.MainItems[i].Definition).Name).Append('}');
            }

            return items.ToString();
        }
    }
}
