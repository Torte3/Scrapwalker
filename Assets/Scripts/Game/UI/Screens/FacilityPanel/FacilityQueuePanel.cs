using System.Collections.Generic;
using Scrapwalker.Core.Outpost;
using Scrapwalker.Core.Outpost.Bots;
using Scrapwalker.Core.Outpost.Facilities;
using Scrapwalker.Core.Outpost.Facilities.Commands;
using Scrapwalker.Core.Root;
using Scrapwalker.Game.Look;
using Scrapwalker.Core.Sessions;
using Scrapwalker.Game.UI.Widgets;
using Scrapwalker.Game.Wording;
using Scrapwalker.Game.Wording.Words;
using UnityEngine;
using UnityEngine.UIElements;

namespace Scrapwalker.Game.UI.Screens.FacilityPanel
{
    internal sealed class FacilityQueuePanel : UIScreen
    {
        private readonly RowList<FacilityTaskRow> _tasks;
        private readonly Label _queueEmpty;
        private readonly ScrollView _scroll;
        private readonly List<string> _botBuffer = new List<string>();
        private readonly ShownValue<(World, int, Facility, int?)> _builtFor = new ShownValue<(World, int, Facility, int?)>();
        private int? _filterTaskID;

        public FacilityQueuePanel(VisualElement root, FacilityTemplates templates) : base(root)
        {
            _tasks = new RowList<FacilityTaskRow>(Q("order-rows"), () => new FacilityTaskRow(templates.Task.Instantiate(), templates.Material), row => row.Element);
            _queueEmpty = Q<Label>("queue-empty");
            _queueEmpty.text = RoomWords.EmptyTasks;
            root.WriteLabel("queue-title", RoomWords.SectionQueue);
            _scroll = Q<ScrollView>("queue");
        }

        public void Reset()
        {
            _filterTaskID = null;
            _scroll.scrollOffset = Vector2.zero;
            _builtFor.Reset();
        }

        public void Refresh(Facility f)
        {
            if (_builtFor.Change((World, World.Revision, f, _filterTaskID)))
            {
                Fill(f);
            }

            for (int i = 0; i < _tasks.Count; i++)
            {
                UpdateTask(f, _tasks[i]);
            }
        }

        private void Fill(Facility f)
        {
            _tasks.Begin();
            foreach (WorkTask task in f.Work.Queue.Tasks)
            {
                BindTask(_tasks.Next(), f, task);
            }

            _tasks.End();
            _queueEmpty.Display(_tasks.Count == 0);
        }

        private void BindTask(FacilityTaskRow row, Facility f, WorkTask task)
        {
            RecipeDefinition recipe = task.Recipe;
            row.Task = task;
            row.Delivered = type => f.Work.FedCount(task, type);
            row.SetTitle(Data.NameOf(recipe));
            row.Materials.ShowWithEnergy(Data, recipe.Inputs, recipe.Energy);
            row.Remove.OnClick(() => Session.Execute(new RemoveWorkTask(f.ID, task.ID)));
            row.OnMove(
                () => Session.Execute(new MoveWorkTask(f.ID, task.ID, MoveDirection.Up)),
                () => Session.Execute(new MoveWorkTask(f.ID, task.ID, MoveDirection.Down)),
                () => Session.Execute(new MoveWorkTask(f.ID, task.ID, MoveDirection.Front)));
            row.Loop.OnClick(recipe.IsOneOff ? null : () => Session.Execute(new SetWorkTaskRepeat(f.ID, task.ID, !task.IsRepeating)));
            row.Filter.OnToggle(() => _filterTaskID = _filterTaskID == task.ID ? null : task.ID);
            row.ApplyFilter = chosen => Session.Execute(new SetInputFilter(f.ID, task.ID, chosen));
        }

        private void CollectBots(Facility f, WorkTask task, List<string> into)
        {
            into.Clear();
            foreach (Bot b in World.Bots)
            {
                BotDoing doing = BotDoingReader.Read(b);
                if (doing.Serves(f, task.ID))
                {
                    into.Add(doing.Verb.Sentence(doing, b, Data));
                }
            }
        }

        private void UpdateTask(Facility f, FacilityTaskRow row)
        {
            WorkTask task = row.Task;
            RecipeDefinition recipe = task.Recipe;
            row.SetWorking(f.Work.WorkingTask == task);
            row.SetMovable(
                Session.Preview(new MoveWorkTask(f.ID, task.ID, MoveDirection.Up)).Own.IsOK,
                Session.Preview(new MoveWorkTask(f.ID, task.ID, MoveDirection.Down)).Own.IsOK,
                Session.Preview(new MoveWorkTask(f.ID, task.ID, MoveDirection.Front)).Own.IsOK);
            row.SetLoop(!recipe.IsOneOff, task.IsRepeating);

            row.SetStatus(f.StatusOf(World, task));

            CollectBots(f, task, _botBuffer);
            row.SetBots(_botBuffer);

            row.Materials.SetCounts(World.Ledger, task.Filter, 1, row.Delivered);

            row.Materials.SetEnergy(Data, recipe.Energy, World.Energy.Stored, f.Work.HasStarted(task));

            row.Bar.Show(World, f, task);
            row.Filter.Refresh(recipe.Inputs.Count > 0, task.Filter, _filterTaskID == task.ID, World.Progress, row.ApplyFilter);
        }
    }
}
