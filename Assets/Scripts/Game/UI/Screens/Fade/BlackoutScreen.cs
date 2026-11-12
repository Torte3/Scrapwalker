using Scrapwalker.Core.Root;
using Scrapwalker.Core.Sessions;
using Scrapwalker.Game.UI.Widgets;
using Scrapwalker.Game.Wording.Words;
using UnityEngine.UIElements;

namespace Scrapwalker.Game.UI.Screens.Fade
{
    public sealed class BlackoutScreen : UIScreen
    {
        private readonly VisualElement _black;
        private readonly VisualElement _line;
        private readonly ShownValue<bool> _blackShown = new ShownValue<bool>();
        private readonly ShownValue<bool> _lineShown = new ShownValue<bool>();
        private readonly ShownValue<float> _alphaShown = new ShownValue<float>();

        public BlackoutScreen(VisualElement root) : base(root)
        {
            _black = Q("blackout");
            _line = Q("defeat-line");
            root.WriteLabel("defeat-text", DefeatWords.Title);
        }

        protected override void OnBind()
        {
            _blackShown.Reset();
            _lineShown.Reset();
            _alphaShown.Reset();
        }

        public void Refresh()
        {
            Blackout blackout = Session.Blackout;
            if (_blackShown.Change(blackout.IsActive))
            {
                _black.Display(blackout.IsActive);
            }

            float alpha = blackout.Darkness;
            if (_alphaShown.Change(alpha))
            {
                _black.style.opacity = alpha;
            }

            bool showsLine = World.IsDefeated;
            if (_lineShown.Change(showsLine))
            {
                _line.Display(showsLine);
            }
        }
    }
}
