using Scrapwalker.Core.Items;
using Scrapwalker.Core.Outpost;
using Scrapwalker.Core.Outpost.Grid;
using Scrapwalker.Core.Root;
using Scrapwalker.Core.Sessions;
using Scrapwalker.Game.Definitions;
using Scrapwalker.Game.Look;
using Scrapwalker.Game.Wording;
using UnityEngine;

namespace Scrapwalker.Game.UI.Build
{
    public sealed class BuildTool
    {
        private readonly Session _session;
        private readonly GameData _data;

        public BuildTool(Session session, GameData data)
        {
            _session = session;
            _data = data;
        }

        public BuildGesture Gesture { get; private set; } = BuildGesture.None;
        public FacilityDefinition PlacingDefinition => Gesture.PlacingDefinition;
        public Facility Relocating => Gesture.Relocating;
        public QualityFilter Filter { get; set; } = QualityFilter.Any;

        public bool IsActive => Gesture.Definition != null;

        public BuildGhost? Ghost { get; private set; }

        public event System.Action Ran;

        public void BeginPlace(FacilityDefinition definition)
        {
            Gesture = new PlaceGesture(definition);
        }

        public void BeginRelocate(Facility facility)
        {
            Gesture = new RelocateGesture(facility);
        }

        public void Cancel()
        {
            Gesture = BuildGesture.None;
            Ghost = null;
        }

        public void Hover(Cell? cursor)
        {
            if (!cursor.HasValue)
            {
                Ghost = null;
                return;
            }

            Cell at = PlaceAt(cursor.Value);
            CommandResult check = _session.Preview(Gesture.CommandAt(at, Filter)).Own;
            string text = check.IsOK ? Gesture.ClickText : check.Text(_data);
            Ghost = new BuildGhost(Gesture.Definition, at, check.IsOK, text);
        }

        public bool TryRun(Cell cursor, bool keepPlacing)
        {
            CommandResult result = _session.Execute(Gesture.CommandAt(PlaceAt(cursor), Filter));
            if (!result.IsOK)
            {
                return false;
            }

            Ran?.Invoke();
            if (!Gesture.ContinuesAfterRun(keepPlacing))
            {
                Cancel();
            }

            return true;
        }

        private Cell PlaceAt(Cell cursor)
        {
            FacilityDefinition definition = Gesture.Definition;
            World world = _session.World;
            int column = Mathf.Clamp(cursor.Column - definition.Width / 2, 0, world.Columns - definition.Width);
            int row = Mathf.Clamp(cursor.Row - definition.Height / 2, 0, world.Rows - definition.Height);
            return new Cell(column, row);
        }
    }
}
