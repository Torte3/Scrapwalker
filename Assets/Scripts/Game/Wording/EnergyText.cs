using UnityEngine;

namespace Scrapwalker.Game.Wording
{
    public static class EnergyText
    {
        public static int Amount(float energy)
        {
            return Mathf.FloorToInt(energy + 0.001f);
        }

        public static string Need(float energy)
        {
            return Texts.Decimal(energy);
        }
    }
}
