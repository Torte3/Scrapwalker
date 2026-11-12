using System.Collections.Generic;
using System.Linq;
using Scrapwalker.Core.Framework;
using Scrapwalker.Core.Items;
using Scrapwalker.Core.Outpost.Bots.Actions;
using Scrapwalker.Core.Outpost.Bots.Haul;
using Scrapwalker.Core.Outpost.Grid;
using Scrapwalker.Core.Root;

namespace Scrapwalker.Core.Outpost.Bots
{
    public sealed class Bot
    {
        private const float SamePlace = 0.05f;

        private readonly List<GridPoint> _path = new List<GridPoint>();

        public int ID { get; }
        public float BatterySeconds { get; internal set; }
        public bool IsLowPower => BatterySeconds <= 0f;
        public GridPoint Position { get; internal set; }
        public BotAction Action { get; internal set; } = new IdleAction();
        public GridPoint Goal { get; internal set; }
        public IReadOnlyList<GridPoint> Path => _path;
        public int PathIndex { get; private set; }
        public ItemStack Cargo { get; internal set; }

        public bool IsLost { get; internal set; }

        public Facility Station { get; internal set; }

        public bool IsIdle => Action.IsIdle;
        public bool IsCarrying => !Cargo.IsEmpty;
        public bool IsMoving => PathIndex < _path.Count;
        public bool IsHeading => !IsIdle && IsMoving;
        public bool IsClimbing => IsMoving && IsVertical(_path[PathIndex]);

        public Bot(int id, GridPoint at)
        {
            ID = id;
            Position = at;
        }

        internal string LogName => "B-" + ID.ToString("00");

        public float BatteryFill(GameConfiguration configuration)
        {
            return BatterySeconds / configuration.BatterySeconds;
        }

        public bool NeedsCharge(GameConfiguration configuration)
        {
            return BatterySeconds < configuration.ChargeThreshold * configuration.BatterySeconds;
        }

        public float Speed(GameConfiguration configuration)
        {
            return IsLowPower ? configuration.LowBatterySpeed : 1f;
        }

        private bool IsVertical(GridPoint next)
        {
            return System.Math.Abs(next.X - Position.X) < SamePlace && System.Math.Abs(next.Y - Position.Y) > SamePlace;
        }

        internal bool TryStart(World world, BotAction action)
        {
            IsLost = !TryHeadTo(world, action, action.SpotFor(world, this));
            if (IsLost)
            {
                return false;
            }

            world.Log.Write(() => LogName + " ← " + action.LogText(world));
            return true;
        }

        internal bool TryHeadTo(World world, BotAction action, GridPoint to)
        {
            if (!PlanPath(world.Paths, to))
            {
                return false;
            }

            Action = action;
            return true;
        }

        private bool PlanPath(Navigator paths, GridPoint to)
        {
            PathIndex = 0;
            if (!paths.FindPath(Position, to, _path))
            {
                return false;
            }

            Goal = to;
            return true;
        }

        internal void PassCorner()
        {
            PathIndex++;
        }

        internal BotMemento CreateMemento()
        {
            return new BotMemento
            {
                ID = ID,
                BatterySeconds = BatterySeconds,
                Position = Position.CreateMemento(),
                Action = Action.CreateMemento(),
                Goal = Goal.CreateMemento(),
                Path = _path.Select(point => point.CreateMemento()).ToList(),
                PathIndex = PathIndex,
                Cargo = Cargo.CreateMemento(),
                Lost = IsLost,
                StationID = Station?.ID,
            };
        }

        internal static Bot Restore(BotMemento memento, World world, LoadContext load)
        {
            var bot = new Bot(memento.ID, GridPoint.Restore(memento.Position))
            {
                BatterySeconds = memento.BatterySeconds,
                Action = memento.Action.Restore(world, load),
                Goal = GridPoint.Restore(memento.Goal),
                Cargo = ItemStack.Restore(memento.Cargo, load),
                IsLost = memento.Lost,
                Station = memento.StationID.HasValue ? world.Facilities.Find(memento.StationID.Value) : null,
            };

            bot._path.AddRange(memento.Path.Select(GridPoint.Restore));
            bot.PathIndex = memento.PathIndex;
            return bot;
        }

        internal bool MayTake(World world, AssignmentAction action)
        {
            if (Station != null)
            {
                return action.Serves(Station);
            }

            return !world.Bots.Any(other => other.Station != null && action.Holds(Place.At(other.Station), PlaceUse.Post));
        }

        internal void ClearCargo()
        {
            Cargo = default;
        }

        internal void Leave(World world)
        {
            HaulSystem.DropOnFloor(world, this);
            GoIdle();
        }

        internal void GoIdle()
        {
            Action = new IdleAction();
            _path.Clear();
            PathIndex = 0;
        }
    }
}
