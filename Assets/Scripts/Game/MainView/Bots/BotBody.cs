using Scrapwalker.Core.Outpost.Bots;
using Scrapwalker.Game.Look;
using Scrapwalker.Game.Look.Art;
using Scrapwalker.Game.Look.Art.Sets;
using UnityEngine;

namespace Scrapwalker.Game.MainView.Bots
{
    public readonly struct BotBody
    {
        public static readonly BotBody Normal = new BotBody(false, false);

        public BotBody(bool isLost, bool isLowPower)
        {
            IsLost = isLost;
            IsLowPower = isLowPower;
        }

        public BotBody(Bot bot)
            : this(bot.IsLost, bot.IsLowPower)
        {
        }

        public bool IsLost { get; }
        public bool IsLowPower { get; }

        public ArtEntry In(BotPoseArt pose)
        {
            if (IsLost)
            {
                return pose.Lost;
            }

            return IsLowPower ? pose.LowPower : pose.Normal;
        }

        public Color Color
        {
            get
            {
                if (IsLost)
                {
                    return Palette.BotLost;
                }

                return IsLowPower ? Palette.BotLowPower : Palette.BotNormal;
            }
        }

        public Color Tint
        {
            get
            {
                Color body = Color;
                return new Color(body.r / Palette.BotNormal.r, body.g / Palette.BotNormal.g, body.b / Palette.BotNormal.b, 1f);
            }
        }
    }
}
