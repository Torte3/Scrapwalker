using System;
using UnityEngine.UIElements;

namespace Scrapwalker.Game.UI.Widgets
{
    public sealed class Meter
    {
        private readonly Label _value;
        private readonly FillBar _fill;
        private readonly Func<int, int, string> _format;
        private readonly ShownValue<(int, int)> _shown = new ShownValue<(int, int)>();

        public Meter(Label value, VisualElement fill, Func<int, int, string> format)
        {
            _value = value;
            _fill = fill != null ? new FillBar(fill) : null;
            _format = format;
        }

        public bool Set(int now, int max)
        {
            return Set(now, max, max > 0 ? (float)now / max : 0f);
        }

        public bool Set(int now, int max, float fill)
        {
            _fill?.Set(fill);
            if (!_shown.Change((now, max)))
            {
                return false;
            }

            _value.text = _format(now, max);
            return true;
        }

        public void Reset()
        {
            _shown.Reset();
            _fill?.Reset();
        }
    }
}
