using System;
using Scrapwalker.Core.Outpost;
using Scrapwalker.Game.Definitions;

namespace Scrapwalker.Game.Wording
{
    public static class FacilityText
    {
        public static string Size(this FacilityDefinition definition)
        {
            int width = definition.Width;
            int height = definition.Height;
            return $"{width}×{height}マス";
        }

        public static string CategoryName(this BuildCategory category)
        {
            return category switch
            {
                BuildCategory.Production => "生産",
                BuildCategory.Weapon => "兵装",
                BuildCategory.Power => "電力",
                BuildCategory.Upkeep => "整備",
                _ => throw new ArgumentOutOfRangeException(nameof(category), category, "建設の分類の名前が無い"),
            };
        }
    }
}
