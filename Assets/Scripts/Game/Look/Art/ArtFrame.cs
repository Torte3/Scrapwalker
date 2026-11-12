using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Scrapwalker.Game.Look.Art
{
    [Serializable]
    public sealed class ArtFrame
    {
        [SerializeField, FormerlySerializedAs("Sprite")] private Sprite _sprite;

        public Sprite Sprite => _sprite;

        public static ArtFrame Of(Sprite sprite)
        {
            return new ArtFrame { _sprite = sprite };
        }
    }
}
