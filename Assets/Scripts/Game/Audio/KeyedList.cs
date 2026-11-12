using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scrapwalker.Game.Audio
{
    [Serializable]
    public sealed class KeyedList<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>
        where TKey : struct, Enum
    {
        [SerializeField] private Keyed<TKey, TValue>[] _entries;

        [NonSerialized] private Dictionary<TKey, TValue> _byKey;

        public TValue this[TKey key]
        {
            get
            {
                if (!ByKey.TryGetValue(key, out TValue value))
                {
                    throw new KeyNotFoundException("音の表に " + typeof(TKey).Name + "." + key + " が無い");
                }

                return value;
            }
        }

        private Dictionary<TKey, TValue> ByKey
        {
            get
            {
                if (_byKey == null)
                {
                    _byKey = new Dictionary<TKey, TValue>();
                    foreach (Keyed<TKey, TValue> entry in _entries)
                    {
                        _byKey[entry.Key] = entry.Value;
                    }
                }

                return _byKey;
            }
        }

        public List<string> Problems()
        {
            var problems = new List<string>();
            var seen = new HashSet<TKey>();
            foreach (Keyed<TKey, TValue> entry in _entries)
            {
                if (!seen.Add(entry.Key))
                {
                    problems.Add(typeof(TKey).Name + "." + entry.Key + " が2つある");
                }
            }

            foreach (TKey key in (TKey[])Enum.GetValues(typeof(TKey)))
            {
                if (!seen.Contains(key))
                {
                    problems.Add(typeof(TKey).Name + "." + key + " が無い");
                }
            }

            return problems;
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return ByKey.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
