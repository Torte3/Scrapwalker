using System;
using Scrapwalker.Core.Items;
using Scrapwalker.Core.Root;
using UnityEngine.UIElements;

namespace Scrapwalker.Game.UI.Widgets.QualityFilters
{
    public sealed class QualityFilterPicker
    {
        private readonly VisualElement _iconSlot;
        private readonly VisualElement _editorSlot;
        private readonly QualityFilterIcon _icon;
        private readonly QualityFilterEditor _editor;

        public QualityFilterPicker(VisualElement iconSlot, VisualElement editorSlot)
        {
            _iconSlot = iconSlot;
            _editorSlot = editorSlot;
            _icon = new QualityFilterIcon(iconSlot);
            _editor = new QualityFilterEditor(editorSlot);
        }

        public bool IsShown { get; private set; }
        public bool IsOpen { get; private set; }

        public void OnToggle(Action toggle)
        {
            _icon.OnClick(toggle);
        }

        public void Refresh(bool hasMaterials, QualityFilter filter, bool open, Progress progress, Action<QualityFilter> apply)
        {
            ShownQualities shown = ShownQualities.Of(progress);
            IsShown = hasMaterials && shown.HasChoice;
            IsOpen = IsShown && open;
            _iconSlot.Display(IsShown);
            _editorSlot.Display(IsOpen);
            if (!IsShown)
            {
                return;
            }

            _icon.Set(filter, IsOpen, shown);
            if (IsOpen)
            {
                _editor.Set(filter, shown, apply);
            }
        }
    }
}
