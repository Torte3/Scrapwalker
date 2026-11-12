using System;
using System.Collections.Generic;

namespace Scrapwalker.Core.Framework
{
    public sealed class GameEvents
    {
        private readonly List<GameEvent> _pending = new List<GameEvent>();
        private readonly Dictionary<Type, List<Action<GameEvent>>> _listeners = new Dictionary<Type, List<Action<GameEvent>>>();

        internal void On<T>(Action<T> listener) where T : GameEvent
        {
            if (!_listeners.TryGetValue(typeof(T), out List<Action<GameEvent>> listeners))
            {
                listeners = new List<Action<GameEvent>>();
                _listeners.Add(typeof(T), listeners);
            }

            listeners.Add(e => listener((T)e));
        }

        internal void Emit(GameEvent gameEvent)
        {
            _pending.Add(gameEvent);
            if (!_listeners.TryGetValue(gameEvent.GetType(), out List<Action<GameEvent>> listeners))
            {
                return;
            }

            foreach (Action<GameEvent> listener in listeners)
            {
                listener(gameEvent);
            }
        }

        internal GameEvent[] Drain()
        {
            if (_pending.Count == 0)
            {
                return Array.Empty<GameEvent>();
            }

            GameEvent[] drained = _pending.ToArray();
            _pending.Clear();
            return drained;
        }
    }
}
