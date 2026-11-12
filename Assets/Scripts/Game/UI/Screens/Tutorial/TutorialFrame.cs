using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Scrapwalker.Game.UI.Screens.Tutorial
{
    public readonly struct TutorialFrame
    {
        public TutorialFrame(IReadOnlyList<Rect> menus, IReadOnlyList<Rect> time, IReadOnlyList<Rect> controls, Rect field, VisualElement showcase)
        {
            Menus = menus;
            Time = time;
            Controls = controls;
            Field = field;
            Showcase = showcase;
        }

        public IReadOnlyList<Rect> Menus { get; }
        public IReadOnlyList<Rect> Time { get; }
        public IReadOnlyList<Rect> Controls { get; }
        public Rect Field { get; }
        public VisualElement Showcase { get; }
    }
}
