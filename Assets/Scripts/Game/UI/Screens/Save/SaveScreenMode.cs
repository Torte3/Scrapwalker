using Scrapwalker.Core.Save;
using Scrapwalker.Core.Sessions;
using Scrapwalker.Game.Wording.Words;

namespace Scrapwalker.Game.UI.Screens.Save
{
    public abstract class SaveScreenMode
    {
        public static readonly SaveScreenMode Menu = new MenuMode();
        public static readonly SaveScreenMode Load = new LoadMode();
        public static readonly SaveScreenMode Ending = new EndingMode();

        internal abstract string Title { get; }

        internal virtual bool OffersNewGame => false;

        internal abstract void FillButtons(SaveWindowScreen screen, SaveRow row, SaveSlot slot);

        internal virtual bool Write(Session session, int slot)
        {
            return false;
        }

        internal virtual void AfterClose(IAppHost app)
        {
        }

        internal virtual void AfterLoad(IAppHost app)
        {
        }

        private sealed class MenuMode : SaveScreenMode
        {
            internal override string Title => SaveWords.Title;

            internal override bool OffersNewGame => true;

            internal override void FillButtons(SaveWindowScreen screen, SaveRow row, SaveSlot slot)
            {
                bool canSave = screen.IsWorldSavable;
                if (!slot.HasSave)
                {
                    if (!slot.IsAuto)
                    {
                        screen.UseSave(row, slot, canSave);
                    }

                    return;
                }

                screen.UseLoad(row, slot, true, false);
                if (!slot.IsAuto)
                {
                    screen.UseSave(row, slot, canSave);
                    screen.UseDelete(row, slot);
                }
            }

            internal override bool Write(Session session, int slot)
            {
                return session.Save(slot);
            }
        }

        private sealed class LoadMode : SaveScreenMode
        {
            internal override string Title => SaveWords.Title;

            internal override void FillButtons(SaveWindowScreen screen, SaveRow row, SaveSlot slot)
            {
                if (slot.HasSave)
                {
                    screen.UseLoad(row, slot, true, true);
                }
            }

            internal override void AfterLoad(IAppHost app)
            {
                app.SlideInFromTitle();
            }
        }

        private sealed class EndingMode : SaveScreenMode
        {
            internal override string Title => SaveWords.TitleEnding;

            internal override void FillButtons(SaveWindowScreen screen, SaveRow row, SaveSlot slot)
            {
                if (!slot.IsAuto)
                {
                    screen.UseSave(row, slot, true);
                }
            }

            internal override bool Write(Session session, int slot)
            {
                return session.SaveBeforeEscape(slot);
            }

            internal override void AfterClose(IAppHost app)
            {
                app.ShowTitle();
            }
        }
    }
}
