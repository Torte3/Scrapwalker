using System;
using Scrapwalker.Core.Items;

namespace Scrapwalker.Game.Wording
{
    public static class QualityText
    {
        public static string Name(this Quality quality)
        {
            return quality switch
            {
                Quality.Plain => string.Empty,
                Quality.Good => "良質",
                Quality.Fine => "上質",
                Quality.High => "高品質",
                Quality.Best => "最高品質",
                _ => throw new ArgumentOutOfRangeException(nameof(quality), quality, "品質の名前が無い"),
            };
        }

        public static string Tier(this Quality quality)
        {
            string name = quality.Name();
            return quality switch
            {
                Quality.Plain => "無印",
                Quality.Good => $"{name} ☆1",
                Quality.Fine => $"{name} ☆2",
                Quality.High => $"{name} ☆3",
                Quality.Best => $"{name} ☆4",
                _ => throw new ArgumentOutOfRangeException(nameof(quality), quality, "品質の段の文が無い"),
            };
        }

        public static string WithStars(this Quality quality)
        {
            return quality == Quality.Plain ? string.Empty : quality.Tier();
        }
    }
}
