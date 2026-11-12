using System.Collections.Generic;
using Scrapwalker.Core.Combat;
using Scrapwalker.Core.Items;
using Scrapwalker.Core.Travel;
using Scrapwalker.Game.Definitions;
using Scrapwalker.Game.UI.Screens.Ticker;
using Scrapwalker.Game.UI.Widgets;
using Scrapwalker.Game.Wording.Words;
using UnityEngine;

namespace Scrapwalker.Game.App
{
    public sealed class EventLog
    {
        private readonly TickerScreen _ticker;
        private readonly GameData _data;
        private PendingEntry _pending;

        public EventLog(TickerScreen ticker, GameData data)
        {
            _ticker = ticker;
            _data = data;
        }

        private sealed class PendingEntry
        {
            private readonly List<(LootKey Key, int Count)> _items = new List<(LootKey Key, int Count)>();

            public PendingEntry(LootSource source, int enemyID, string text)
            {
                Source = source;
                EnemyID = enemyID;
                Text = text;
            }

            public LootSource Source { get; }
            public int EnemyID { get; }
            public string Text { get; }

            public void Add(LootKey key, int count)
            {
                int index = _items.FindIndex(item => item.Key.Equals(key));
                if (index < 0)
                {
                    _items.Add((key, count));
                    return;
                }

                _items[index] = (key, _items[index].Count + count);
            }

            public IReadOnlyList<LootItem> Items()
            {
                return _items.ConvertAll(item => new LootItem(item.Key.Type, item.Key.Quality, item.Count));
            }
        }

        private struct LootKey
        {
            public MaterialDefinition Type;
            public Quality Quality;
        }

        public void Listen(GameEventListeners listeners)
        {
            listeners.On<EnemyKilled>(OnEnemyKilled);
            listeners.On<Looted>(OnLooted);
        }

        private void OnEnemyKilled(EnemyKilled killed)
        {
            Begin(LootSource.Enemy, killed.EnemyID, LogWords.Killed(_data.NameOf(killed.Enemy.Type)));
        }

        private void OnLooted(Looted looted)
        {
            if (_pending == null || _pending.Source != looted.Source || _pending.EnemyID != looted.EnemyID)
            {
                Begin(looted.Source, looted.EnemyID, looted.Source == LootSource.Enemy ? LogWords.EnemyDrop : LogWords.WaveDig(looted.WaveNumber));
            }

            ItemStack items = looted.Items;
            _pending.Add(new LootKey { Type = items.Type, Quality = items.Quality }, Mathf.Max(1, items.Count));
        }

        private void Begin(LootSource source, int enemyID, string text)
        {
            Flush();
            _pending = new PendingEntry(source, enemyID, text);
        }

        public void Flush()
        {
            if (_pending == null)
            {
                return;
            }

            PendingEntry entry = _pending;
            _pending = null;
            _ticker.Push(entry.Text, entry.Items());
        }
    }
}
