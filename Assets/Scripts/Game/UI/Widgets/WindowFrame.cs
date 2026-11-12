using System;
using UnityEngine.UIElements;

namespace Scrapwalker.Game.UI.Widgets
{
    public sealed class WindowFrame
    {
        private readonly VisualElement _layer;
        private readonly Label _title;
        private readonly Button _close;
        private Action _onClose;

        public WindowFrame(VisualElement instance)
        {
            _layer = instance.Need<VisualElement>("window-layer");
            Frame = _layer.Need<VisualElement>("frame");
            HeadExtra = _layer.Need<VisualElement>("head-extra");
            _title = _layer.Need<Label>("title");
            _close = _layer.Need<Button>("close");
            _close.clicked += InvokeCloseAction;
            _layer.Need<VisualElement>("backdrop").RegisterCallback<ClickEvent>(_ => InvokeCloseAction());
            _layer.Display(false);
        }

        public VisualElement Frame { get; }
        public VisualElement HeadExtra { get; }
        public Button CloseButton => _close;
        public bool IsOpen => _layer.IsShown();

        public void Open(string title)
        {
            _title.text = title;
            _layer.Display(true);
        }

        public void SetTitle(string title)
        {
            _title.text = title;
        }

        public void Close()
        {
            _layer.Display(false);
        }

        public void ShowHeadExtra(bool on)
        {
            HeadExtra.Display(on);
            _title.style.flexGrow = on ? 0f : 1f;
        }

        public void SetCloseAction(Action action)
        {
            _onClose = action;
            Frame.EnableInClassList("window--no-close", action == null);
        }

        private void InvokeCloseAction()
        {
            _onClose?.Invoke();
        }
    }
}
