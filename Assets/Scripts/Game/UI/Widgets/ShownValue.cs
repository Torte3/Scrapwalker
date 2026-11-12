using System.Collections.Generic;

namespace Scrapwalker.Game.UI.Widgets
{
    public sealed class ShownValue<T>
    {
        private bool _isWritten;
        private T _value;

        public bool Change(T value)
        {
            if (_isWritten && EqualityComparer<T>.Default.Equals(_value, value))
            {
                return false;
            }

            _isWritten = true;
            _value = value;
            return true;
        }

        public void Reset()
        {
            _isWritten = false;
        }
    }
}
