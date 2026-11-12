using System;
using System.Linq;
using Scrapwalker.Core.Outpost;
using Scrapwalker.Core.Root;
using Scrapwalker.Core.Travel;
using Scrapwalker.Game.UI.Widgets;
using Scrapwalker.Game.Wording;
using Scrapwalker.Game.Wording.Words;
using UnityEngine;
using UnityEngine.UIElements;

namespace Scrapwalker.Game.UI.Screens.Clear
{
    public sealed class ClearScreen : UIScreen
    {
        private readonly VisualElement _clear;

        public ClearScreen(VisualElement root) : base(root)
        {
            _clear = Q("clear");
            Q<Button>("next").OnClick(PressNext);
            root.WriteLabel("title-label", ClearWords.Title);
            root.WriteLabel("time-label", ClearWords.Time);
            root.WriteLabel("ruins-label", ClearWords.Ruins);
            root.WriteLabel("research-label", ClearWords.Research);
            root.WriteLabel("rooms-label", ClearWords.Rooms);
            root.WriteLabel("bots-label", ClearWords.Bots);
            root.WriteLabel("kills-label", ClearWords.Kills);
            root.WriteLabel("materials-label", ClearWords.Materials);
            root.WriteLabel("waves-label", ClearWords.Waves);
            root.WriteLabel("next-label", ClearWords.Next);
        }

        public Action AfterNext { get; set; }

        public bool IsOpen { get; private set; }

        protected override void OnBind()
        {
            Close();
        }

        public void Open()
        {
            IsOpen = true;
            World world = World;
            Progress progress = world.Progress;
            SetNumber("time", Texts.Hours(Mathf.RoundToInt(world.TickCount * world.Configuration.TickSeconds)));
            RuinDefinition[] ruins = world.Definitions.Ruins.Where(ruin => ruin.Plan.CanBeCleared).ToArray();
            SetCount("ruins", ruins.Count(world.Exploration.HasCleared), ruins.Length);
            SetCount("research", world.Definitions.Research.Count(world.Research.IsDone), world.Definitions.Research.Count);
            SetNumber("rooms", world.Facilities.Count(IsBuilt));
            SetNumber("bots", world.Bots.Count);
            SetNumber("kills", progress.EnemiesKilled);
            SetNumber("materials", progress.MaterialsCollected);
            SetNumber("waves", progress.WavesHeld);
            _clear.Display(true);
        }

        public void Close()
        {
            IsOpen = false;
            _clear.Display(false);
        }

        private void PressNext()
        {
            Close();
            AfterNext?.Invoke();
        }

        private static bool IsBuilt(Facility f)
        {
            return !f.Condition.IsUnderConstruction || f.Condition.IsRebuilding;
        }

        private void SetCount(string name, int done, int total)
        {
            SetNumber(name, done);
            Q<Label>(name + "-total").text = ClearWords.Total(Texts.Monospace(total));
        }

        private void SetNumber(string name, object value)
        {
            Q<Label>(name).text = Texts.Monospace(value);
        }
    }
}
