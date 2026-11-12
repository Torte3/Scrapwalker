using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace Scrapwalker.Game.UI.Widgets
{
    public sealed class RowList<T>
    {
        private readonly VisualElement _parent;
        private readonly Func<T> _make;
        private readonly Func<T, VisualElement> _element;
        private readonly List<T> _rows = new List<T>();

        public RowList(VisualElement parent, Func<T> make, Func<T, VisualElement> element)
        {
            _parent = parent;
            _make = make;
            _element = element;
        }

        public int Count { get; private set; }

        public T this[int index] => _rows[index];

        public void SetCount(int count)
        {
            Begin();
            for (int i = 0; i < count; i++)
            {
                Next();
            }

            End();
        }

        public void Begin()
        {
            Count = 0;
        }

        public T Next()
        {
            if (Count == _rows.Count)
            {
                T made = _make();
                _parent.Add(_element(made));
                _rows.Add(made);
            }

            T row = _rows[Count++];
            _element(row).Display(true);
            return row;
        }

        public void End()
        {
            for (int i = Count; i < _rows.Count; i++)
            {
                _element(_rows[i]).Display(false);
            }
        }
    }
}
