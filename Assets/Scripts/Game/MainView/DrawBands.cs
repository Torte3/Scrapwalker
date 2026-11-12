using System;
using UnityEngine;

namespace Scrapwalker.Game.MainView
{
    public static class DrawBands
    {
        private static readonly int[] _layerIDs;

        static DrawBands()
        {
            _layerIDs = LayerIDs();
        }

        public static int LayerID(this DrawBand band)
        {
            return _layerIDs[(int)band];
        }

        public static int UniqueLayerID(this DrawBand band)
        {
            return Animator.StringToHash("Scrapwalker.SortingLayer." + band) & 0x7fffffff;
        }

        private static int[] LayerIDs()
        {
            Array bands = Enum.GetValues(typeof(DrawBand));
            var ids = new int[bands.Length];
            foreach (DrawBand band in bands)
            {
                ids[(int)band] = band.UniqueLayerID();
            }

            return ids;
        }
    }
}
