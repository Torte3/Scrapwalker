using System;
using System.Collections.Generic;
using Scrapwalker.Core.Framework;

namespace Scrapwalker.Game.App
{
    public sealed class GameEventListeners
    {
        private readonly Dictionary<Type, List<Action<GameEvent>>> _listeners = new Dictionary<Type, List<Action<GameEvent>>>();

        public void On<T>(Action<T> listener) where T : GameEvent
        {
            if (!_listeners.TryGetValue(typeof(T), out List<Action<GameEvent>> listeners))
            {
                listeners = new List<Action<GameEvent>>();
                _listeners.Add(typeof(T), listeners);
            }

            listeners.Add(gameEvent => listener((T)gameEvent));
        }

        public void Dispatch(GameEvent gameEvent)
        {
            if (!_listeners.TryGetValue(gameEvent.GetType(), out List<Action<GameEvent>> listeners))
            {
                return;
            }

            foreach (Action<GameEvent> listener in listeners)
            {
                listener(gameEvent);
            }
        }
    }
}
