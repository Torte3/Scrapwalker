using System;
using UnityEngine;

namespace Scrapwalker.Game.Audio
{
    [Serializable]
    public sealed class Keyed<TKey, TValue>
        where TKey : struct, Enum
    {
        [SerializeField] private TKey _key;
        [SerializeField] private TValue _value;

        public TKey Key => _key;
        public TValue Value => _value;
    }
}
