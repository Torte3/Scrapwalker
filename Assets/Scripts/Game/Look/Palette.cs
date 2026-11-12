using Scrapwalker.Core.Items;
using UnityEngine;

namespace Scrapwalker.Game.Look
{
    public static class Palette
    {
        public static readonly Color Background = Hex("#050605");
        public static readonly Color Amber = Hex("#e8a33d");
        public static readonly Color Red = Hex("#e0533d");
        public static readonly Color Cyan = Hex("#4cc9e0");
        public static readonly Color Scrap = Hex("#a58a5c");
        public static readonly Color OK = Hex("#8fd694");
        public static readonly Color Spark = Hex("#f2e27a");
        public static readonly Color Ember = Hex("#f2a04c");
        public static readonly Color Debris = Hex("#6d6a58");
        public static readonly Color SpaceBlack = Hex("#04060a");
        public static readonly Color StarLight = new Color(230f / 255f, 236f / 255f, 1f);
        public static readonly Color Smoke = RGBA(200, 190, 170, 0.18f);
        public static readonly Color Steel = Hex("#9fb8cc");
        public static readonly Color BoosterSeam = RGB(150, 230, 255);
        public static readonly Color BoosterLight = RGB(110, 215, 255);
        public static readonly Color BoosterCharged = RGB(220, 245, 255);
        public static readonly Color BoosterArc = RGBA(200, 245, 255, 0.9f);
        public static readonly Color BoosterMote = RGB(170, 235, 255);
        public static readonly Color BoosterSpark = RGB(255, 200, 90);
        public static readonly Color ExhaustFull = RGB(255, 140, 60);
        public static readonly Color ExhaustWarm = RGB(255, 90, 60);
        public static readonly Color LaunchStrut = Hex("#3a4148");
        public static readonly Color LaunchFlash = RGB(235, 250, 255);
        public static readonly Color LaunchScrape = RGB(255, 220, 90);
        public static readonly Color LaunchBuild = RGB(140, 220, 255);
        public static readonly Color LaunchFlame = RGB(255, 170, 80);
        public static readonly Color SpeedLine = RGB(230, 225, 200);
        public static readonly Color LabelBack = RGBA(6, 8, 10, 0.62f);
        public static readonly Color UnderGround = Hex("#1b1813");
        public static readonly Color BotNormal = Hex("#8e9680");
        public static readonly Color BotLowPower = Hex("#848c78");
        public static readonly Color BotLost = Hex("#687062");
        public static readonly Color BatteryFrame = Hex("#aeb3a8");
        public static readonly Color BatteryEmptyDim = Hex("#6a2c24");
        public static readonly Color DropShadow = RGBA(120, 112, 96, 0.45f);
        public static readonly Color PuffSteam = RGB(205, 208, 196);
        public static readonly Color PuffSmoke = RGB(60, 62, 52);
        public static readonly Color BlastLight = new Color(1f, 0.7f, 0.35f, 0.9f);
        public static readonly Color BarrierFilm = RGBA(140, 150, 255, 0.10f);
        public static readonly Color BarrierLayer = RGBA(150, 165, 255, 0.55f);
        public static readonly Color BarrierFlash = RGBA(235, 240, 255, 0.95f);
        public static readonly Color DamageNumber = Hex("#fff4dc");
        public static readonly Color BlockedNumber = RGBA(180, 188, 255, 1f);
        public static readonly Color HPBarBack = RGBA(12, 10, 8, 0.7f);
        public static readonly Color HPBarFill = Hex("#e0563b");
        public static readonly Color LockOnEdge = RGBA(0, 0, 0, 0.65f);
        public static readonly Color ArmorSpark = Hex("#d6e4ee");
        public static readonly Color HealNumber = Hex("#86f0a4");
        public static readonly Color HealGlow = RGBA(90, 240, 140, 1f);
        public static readonly Color HealCore = RGBA(200, 255, 215, 1f);
        public static readonly Color BuildOutside = RGBA(5, 7, 6, 0.45f);
        public static readonly Color BuildGrid = Cyan.WithAlpha(0.25f);
        public static readonly Color BuildFreeCell = OK.WithAlpha(0.12f);
        public static readonly Color BuildNoteBack = RGBA(12, 16, 14, 0.92f);
        public static readonly Color[] DigDust = { RGB(110, 102, 80), RGB(88, 82, 66), RGB(126, 116, 92) };
        public static readonly Color PulledDebris = Hex("#55584c");
        public static readonly Color FarFootDust = Hex("#4f4a38");
        public static readonly Color NearFootDust = Hex("#6d6448");
        public static readonly Color PileCount = Hex("#d7dbd2");
        public static readonly Color OverPicture = new Color(1f, 1f, 1f, 0.15f);
        public static readonly Color DismantleShade = RGBA(12, 14, 16, 0.25f);
        public static readonly Color TaskName = Hex("#e8c07d");
        public static readonly Color FacilityName = Hex("#bcc5b6");
        public static readonly Color Repairing = Hex("#ff7a62");
        public static readonly Color Dismantling = Hex("#ff9a80");
        public static readonly Color DismantleBar = Hex("#d98a74");
        public static readonly Color QualityText = Hex("#111111");
        public static readonly Color BarBack = RGBA(12, 16, 14, 0.85f);
        public static readonly Color RelocateFill = Cyan.WithAlpha(0.08f);

        public static readonly BarColors ActiveBar = new BarColors(Amber, Hex("#2f7d8c"));
        public static readonly BarColors IdleBar = new BarColors(Hex("#6f7a68"), Hex("#2a5f68"));

        private static readonly Color[] QualityColors = { Hex("#aeb3a8"), Hex("#8fd694"), Hex("#5aa9e6"), Hex("#f2c94c"), Hex("#c38cf0") };

        public static Color Quality(Quality quality)
        {
            return QualityColors[(int)quality];
        }

        public static Color RGB(int r, int g, int b)
        {
            return RGBA(r, g, b, 1f);
        }

        public static Color RGBA(int r, int g, int b, float a)
        {
            return new Color(r / 255f, g / 255f, b / 255f, a);
        }

        public static Color WithAlpha(this Color color, float alpha)
        {
            color.a = alpha;
            return color;
        }

        public static Color Hex(string hex)
        {
            return ColorUtility.TryParseHtmlString(hex, out Color parsed) ? parsed : Color.magenta;
        }
    }
}
