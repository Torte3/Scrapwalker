using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace Scrapwalker.Game.UI.Widgets
{
    public sealed class ButtonChoice<TKey>
    {
        private readonly string _selectedClass;
        private readonly Action<TKey> _chosen;
        private readonly List<Choice> _choices = new List<Choice>();

        public ButtonChoice(string selectedClass, Action<TKey> chosen)
        {
            _selectedClass = selectedClass;
            _chosen = chosen;
        }

        private readonly struct Choice
        {
            public Choice(TKey key, Button button, Action<bool> shown)
            {
                Key = key;
                Button = button;
                Shown = shown;
            }

            public TKey Key { get; }
            public Button Button { get; }
            public Action<bool> Shown { get; }
        }

        public IEnumerable<Button> Buttons
        {
            get
            {
                foreach (Choice choice in _choices)
                {
                    yield return choice.Button;
                }
            }
        }

        public void Add(TKey key, Button button, Action<bool> shown = null)
        {
            button.clicked += () => _chosen(key);
            _choices.Add(new Choice(key, button, shown));
        }

        public Button ButtonOf(TKey key)
        {
            return _choices.Find(choice => EqualityComparer<TKey>.Default.Equals(choice.Key, key)).Button;
        }

        public void Select(TKey key)
        {
            foreach (Choice choice in _choices)
            {
                bool on = EqualityComparer<TKey>.Default.Equals(choice.Key, key);
                choice.Button.EnableInClassList(_selectedClass, on);
                choice.Shown?.Invoke(on);
            }
        }
    }
}
