using System;
using Scrapwalker.Core.Root;
using Scrapwalker.Core.Save;
using Scrapwalker.Core.Sessions;
using Scrapwalker.Core.Travel;
using Scrapwalker.Game.Audio;
using Scrapwalker.Game.UI.Widgets;
using Scrapwalker.Game.Wording;
using Scrapwalker.Game.Wording.Words;
using UnityEngine;
using UnityEngine.UIElements;

namespace Scrapwalker.Game.UI.Screens.Save
{
    public sealed class SaveWindowScreen : UIScreen
    {
        private enum RowQuestion
        {
            Overwrite,
            Delete,
        }

        private const string Plain = "plain";
        private const string Amber = "amber";
        private const string AmberBold = "amber-bold";
        private const string Danger = "danger";

        private readonly WindowFrame _frame;
        private readonly SaveRow[] _rows = new SaveRow[SaveStore.SlotCount];
        private readonly Button _start;
        private readonly Label _startLabel;
        private SaveSlot[] _slots;
        private bool _isStale;
        private bool _isAskingNewGame;
        private (RowQuestion Question, int Slot)? _rowQuestion;

        public SaveWindowScreen(VisualElement root) : base(root)
        {
            _frame = new WindowFrame(Q("save-window"));
            Q("window-layer").AddToClassList("window-layer--full");
            VisualTreeAsset rowAsset = Root.TakeTemplate("save-row-template");
            VisualElement rows = Q("rows");
            for (int i = 0; i < _rows.Length; i++)
            {
                TemplateContainer row = rowAsset.Instantiate();
                rows.Add(row);
                _rows[i] = new SaveRow(row);
            }

            _start = Q<Button>("start");
            _startLabel = Q<Label>(_start, "start-label");
            _start.OnClick(PressStart);
            _frame.SetCloseAction(PressClose);
        }

        public SaveScreenMode Mode { get; private set; }

        public bool IsOpen => Mode != null;

        internal bool IsWorldSavable => World.CanSave;

        protected override void OnBind()
        {
            Close();
        }

        public void Open(SaveScreenMode mode)
        {
            Mode = mode;
            ClearQuestions();
            _slots = Session.Saves.Describe();
            _frame.Open(mode.Title);
            _start.Show(mode.OffersNewGame);
        }

        public void Close()
        {
            Mode = null;
            ClearQuestions();
            _frame.Close();
        }

        public void Refresh()
        {
            if (!IsOpen || !_isStale)
            {
                return;
            }

            _isStale = false;
            for (int i = 0; i < _rows.Length; i++)
            {
                SetRow(_rows[i], _slots[i]);
            }

            _startLabel.text = _isAskingNewGame ? SaveWords.StartConfirm : SaveWords.Start;
        }

        private void PressClose()
        {
            SaveScreenMode mode = Mode;
            Close();
            mode?.AfterClose(App);
        }

        private void PressStart()
        {
            if (_isAskingNewGame)
            {
                Session.ReplaceWithNewGame();
                Close();
                return;
            }

            _isAskingNewGame = true;
            _rowQuestion = null;
            _isStale = true;
        }

        private void SetRow(SaveRow row, SaveSlot slot)
        {
            RowQuestion? question = _rowQuestion.HasValue && _rowQuestion.Value.Slot == slot.Index ? _rowQuestion.Value.Question : (RowQuestion?)null;
            row.SetFrame(slot.HasSave, slot.IsAuto);
            row.SetName(slot.SlotName(), slot.HasSave);
            row.SetEscaped(slot.HasSave && slot.HasEscaped);
            row.ShowMetadata(slot.HasSave && !question.HasValue);
            if (slot.HasSave && !slot.HasHeading)
            {
                row.SetMetadata(string.Empty, string.Empty, string.Empty);
            }
            else if (slot.HasSave)
            {
                row.SetMetadata(slot.SavedAt.ToString("M/d HH:mm"), ProgressText(slot),Texts.Monospace(Texts.Minutes(Mathf.RoundToInt(slot.Ticks * World.Configuration.TickSeconds))));
            }

            row.HideButtons();
            if (question.HasValue)
            {
                FillQuestionButtons(row, slot, question.Value);
                return;
            }

            row.SetNote(SaveWords.Empty, false);
            Mode.FillButtons(this, row, slot);
        }

        internal void UseLoad(SaveRow row, SaveSlot slot, bool isUsable, bool isMain)
        {
            row.Use(SaveButtonSlot.Left, SaveWords.Load, isMain ? AmberBold : Plain, isUsable, () => Load(slot.Index));
        }

        internal void UseSave(SaveRow row, SaveSlot slot, bool isUsable)
        {
            row.Use(SaveButtonSlot.Middle, SaveWords.Save, Plain, isUsable, slot.HasSave ? () => AskInRow(RowQuestion.Overwrite, slot.Index) : (Action)(() => Save(slot.Index)));
        }

        internal void UseDelete(SaveRow row, SaveSlot slot)
        {
            row.Use(SaveButtonSlot.Right, SaveWords.Delete, Danger, true, () => AskInRow(RowQuestion.Delete, slot.Index));
        }

        private void FillQuestionButtons(SaveRow row, SaveSlot slot, RowQuestion question)
        {
            bool isDeleting = question == RowQuestion.Delete;
            row.SetNote(isDeleting ? SaveWords.AskDelete : SaveWords.AskOverwrite, true);
            row.Use(SaveButtonSlot.Middle, SaveWords.Yes, isDeleting ? Danger : Amber, true, () =>
            {
                if (isDeleting)
                {
                    Session.Saves.Delete(slot.Index);
                    AfterWrite();
                }
                else
                {
                    Save(slot.Index);
                }
            });
            row.Use(SaveButtonSlot.Right, SaveWords.No, Plain, true, ClearQuestions);
        }

        private void Save(int slot)
        {
            if (Mode.Write(Session, slot))
            {
                App.PlaySound(UISound.Saved);
            }

            AfterWrite();
        }

        private void AfterWrite()
        {
            _slots = Session.Saves.Describe();
            ClearQuestions();
        }

        private void Load(int slot)
        {
            SaveScreenMode mode = Mode;
            if (Session.Load(slot))
            {
                Close();
                mode.AfterLoad(App);
            }
        }

        private void AskInRow(RowQuestion question, int slot)
        {
            _isAskingNewGame = false;
            _rowQuestion = (question, slot);
            _isStale = true;
        }

        private void ClearQuestions()
        {
            _isAskingNewGame = false;
            _rowQuestion = null;
            _isStale = true;
        }

        private string ProgressText(SaveSlot slot)
        {
            if (slot.HasEscaped)
            {
                return SaveWords.Cleared;
            }

            RuinDefinition ruin = World.Definitions.ByID<RuinDefinition>(slot.FurthestRuin);
            return ruin != null ? Data.NameOf(ruin) : SaveWords.NoRuin;
        }
    }
}
