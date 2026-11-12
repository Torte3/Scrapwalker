using UnityEngine.UIElements;

namespace Scrapwalker.Game.UI.Widgets
{
    public sealed class FillBar
    {
        private readonly VisualElement _fill;
        private readonly ShownValue<float> _shown = new ShownValue<float>();

        public FillBar(VisualElement fill)
        {
            _fill = fill;
        }

        public void Set(float fraction)
        {
            if (_shown.Change(fraction))
            {
                _fill.SetFill(0f, fraction);
            }
        }

        public void Reset()
        {
            _shown.Reset();
        }
    }
}
