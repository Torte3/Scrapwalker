using Scrapwalker.Game.Audio;
using Scrapwalker.Game.UI.Screens.Tutorial;

namespace Scrapwalker.Game.UI
{
    public interface IAppHost
    {
        float UIDelta { get; }
        float GameDelta { get; }

        bool IsCinematic { get; }

        TutorialCue Cue { get; }
        bool IsExplaining { get; }

        IGridOnScreen GridOnScreen { get; }
        VolumeSettings Volumes { get; }

        void PlaySound(UISound sound);

        void ShowTitle();
        void SlideInFromTitle();
        void Quit();
    }
}
