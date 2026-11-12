using Scrapwalker.Core.Sessions.Tutorial;

namespace Scrapwalker.Game.Look
{
    public static class TutorialLook
    {
        public static float FocusWeight(this TutorialFlow tutorial)
        {
            return Blend(tutorial, TutorialView.Facility);
        }

        public static float EnemiesWeight(this TutorialFlow tutorial)
        {
            return Blend(tutorial, TutorialView.Enemies);
        }

        private static float Blend(TutorialFlow tutorial, TutorialView view)
        {
            if (!tutorial.IsActive)
            {
                return 0f;
            }

            float from = tutorial.ViewFrom == view ? 1f : 0f;
            float to = tutorial.View == view ? 1f : 0f;
            return from + (to - from) * Pen.Smooth(tutorial.PanProgress);
        }
    }
}
