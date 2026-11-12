using Scrapwalker.Game.UI.Widgets;
using UnityEngine;
using UnityEngine.UIElements;

namespace Scrapwalker.Game.UI.Screens.Tutorial
{
    public sealed class WidgetTarget : TutorialTarget
    {
        public WidgetTarget(VisualElement element)
        {
            Element = element;
        }

        public VisualElement Element { get; }

        public override Rect? AreaOn(TutorialScreen screen)
        {
            return Element.IsLaidOut() ? Element.worldBound : (Rect?)null;
        }

        public override bool IsShown => Element.IsLaidOut();
    }
}
