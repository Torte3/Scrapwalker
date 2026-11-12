using System;
using Scrapwalker.Game.UI.Widgets;
using Scrapwalker.Game.Wording.Words;
using UnityEngine.UIElements;

namespace Scrapwalker.Game.UI.Screens.Save
{
    public sealed class SaveRow
    {
        public const int ButtonCount = 3;

        private readonly VisualElement _row;
        private readonly VisualElement _stripe;
        private readonly Label _name;
        private readonly Label _star;
        private readonly VisualElement _metadata;
        private readonly Label _savedAt;
        private readonly Label _ruin;
        private readonly Label _play;
        private readonly Label _note;
        private readonly Button[] _buttons = new Button[ButtonCount];
        private readonly Label[] _labels = new Label[ButtonCount];

        public SaveRow(VisualElement instance)
        {
            _row = instance.Need<VisualElement>("row");
            _stripe = _row.Need<VisualElement>("stripe");
            _name = _row.Need<Label>("name");
            _star = _row.Need<Label>("star");
            _metadata = _row.Need<VisualElement>("meta");
            _savedAt = _metadata.Need<Label>("saved-at");
            _ruin = _metadata.Need<Label>("ruin");
            _play = _metadata.Need<Label>("play");
            _star.text = SaveWords.EscapedMark;
            _metadata.WriteLabel("play-label", SaveWords.Play);
            _note = _row.Need<Label>("note");
            for (int i = 0; i < ButtonCount; i++)
            {
                _buttons[i] = _row.Need<Button>("button-" + i);
                _labels[i] = _buttons[i].Need<Label>("label");
            }
        }

        public void SetFrame(bool hasSave, bool isAuto)
        {
            _row.EnableInClassList("save-row--saved", hasSave);
            _row.EnableInClassList("dash-frame", !hasSave);
            _stripe.Display(isAuto);
        }

        public void SetName(string text, bool hasSave)
        {
            _name.text = text;
            _name.EnableInClassList("dim", !hasSave);
        }

        public void SetEscaped(bool hasEscaped)
        {
            _star.EnableInClassList("invisible", !hasEscaped);
        }

        public void SetMetadata(string savedAt, string ruin, string play)
        {
            _savedAt.text = savedAt;
            _ruin.text = ruin;
            _play.text = play;
        }

        public void ShowMetadata(bool isShown)
        {
            _metadata.Display(isShown);
            _note.Display(!isShown);
        }

        public void SetNote(string text, bool isAsking)
        {
            _note.text = text;
            _note.EnableInClassList("dim", !isAsking);
            _note.EnableInClassList("save-row__note--asking", isAsking);
        }

        public void HideButtons()
        {
            foreach (Button button in _buttons)
            {
                button.Show(false);
                button.OnClick(null);
            }
        }

        public void Use(SaveButtonSlot slot, string label, string variant, bool isUsable, Action action)
        {
            Button button = _buttons[(int)slot];
            button.Show(true);
            _labels[(int)slot].text = label;
            button.SetVariant("save-row__btn--", variant);
            button.SetUsable(isUsable, action);
        }
    }
}
