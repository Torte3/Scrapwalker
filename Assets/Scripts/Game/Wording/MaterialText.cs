using System.Collections.Generic;
using Scrapwalker.Core.Items;
using Scrapwalker.Game.Definitions;
using Scrapwalker.Game.UI;

namespace Scrapwalker.Game.Wording
{
    public static class MaterialText
    {
        private const string Hidden = "<alpha=#00>";
        private const string Clear = "#FFFFFF00";

        public static string Name(this MaterialDefinition type, GameData data)
        {
            return Icon(data, type) + Plain(data, type);
        }

        private static string Plain(GameData data, MaterialDefinition type)
        {
            return data.Asset(type).Name;
        }

        public static string Energy(GameData data)
        {
            return Sprite(data.EnergyTextSprite) + "電気";
        }

        public static string Have(int have)
        {
            return $"/{have}";
        }

        public static string Shortage(this IReadOnlyList<MaterialAmount> lacking, GameData data)
        {
            var parts = new List<string>();
            foreach (MaterialAmount a in lacking)
            {
                string name = a.Type.Name(data);
                int count = a.Count;
                parts.Add($"{name} {count}");
            }

            return string.Join("・", parts);
        }

        public static string Marked(this string source, GameData data, int shown)
        {
            return TextMarks.Expand(source, shown, (name, hidden) => IconOf(data, name, hidden), Hidden);
        }

        private static string Icon(GameData data, MaterialDefinition type)
        {
            return Sprite(data.Asset(type).TextSprite);
        }

        private static string IconOf(GameData data, string name, bool hidden)
        {
            foreach (MaterialDefinition type in data.Definitions.Materials)
            {
                if (data.Asset(type).Name == name)
                {
                    return hidden ? Sprite(data.Asset(type).TextSprite, Clear) : Icon(data, type);
                }
            }

            return null;
        }

        private static string Sprite(int index)
        {
            return "<sprite index=" + index + ">";
        }

        private static string Sprite(int index, string color)
        {
            return "<sprite index=" + index + " color=" + color + ">";
        }
    }
}
