using System;
using Scrapwalker.Core.Save;
using Scrapwalker.Game.UI.Widgets;
using Scrapwalker.Game.Wording.Words;
using UnityEngine;
using UnityEngine.UIElements;

namespace Scrapwalker.Game.UI.Screens.Title
{
    public sealed class TitleScreen : UIScreen
    {
        private readonly VisualElement _title;
        private readonly Button _continue;
        private readonly Button _load;

        public TitleScreen(VisualElement root) : base(root)
        {
            _title = Q("title");
            _continue = Q<Button>("continue");
            _load = Q<Button>("load");
            _continue.OnClick(ContinueFromAutoSave);
            _load.OnClick(() => OpenSaves?.Invoke());
            Q<Button>("new-game").OnClick(() => Session.FadeToNewGame());
            Q<Button>("quit").OnClick(() => App.Quit());
            root.WriteLabel("continue-label", TitleWords.Continue);
            root.WriteLabel("load-label", TitleWords.Load);
            root.WriteLabel("new-game-label", TitleWords.NewGame);
            root.WriteLabel("quit-label", TitleWords.Quit);
        }

        public Action OpenSaves { get; set; }

        public bool IsOpen { get; private set; }

        protected override void OnBind()
        {
            SetArt(Q("title-logo"), Data.Art.Screen.TitleLogo);
            SetArt(Q("title-vignette"), Data.Art.Screen.TitleVignette);
            Close();
        }

        public void Open()
        {
            IsOpen = true;
            SaveStore saves = Session.Saves;
            _continue.Display(saves.Exists(SaveStore.AutoSlot));
            _load.Display(saves.AnyExists());
            _title.Display(true);
        }

        public void Close()
        {
            IsOpen = false;
            _title.Display(false);
        }

        private void ContinueFromAutoSave()
        {
            if (Session.Load(SaveStore.AutoSlot))
            {
                App.SlideInFromTitle();
            }
        }

        private static void SetArt(VisualElement element, Sprite art)
        {
            element.style.backgroundImage = new StyleBackground(art);
        }
    }
}
