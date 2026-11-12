using UnityEngine;

namespace Scrapwalker.Game.UI.Screens.Tutorial
{
    public abstract class TutorialTarget
    {
        public abstract Rect? AreaOn(TutorialScreen screen);

        public abstract bool IsShown { get; }
    }
}
