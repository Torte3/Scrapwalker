using System.Collections.Generic;
using System.Linq;
using Scrapwalker.Core.Outpost.Bots.Assignments;
using Scrapwalker.Core.Root;

namespace Scrapwalker.Core.Outpost.Bots
{
    public sealed class BotAssigner
    {
        private const int MaxAssignments = 256;

        private readonly List<AssignmentAction> _actions = new List<AssignmentAction>();
        private readonly HashSet<Bot> _tried = new HashSet<Bot>();
        private string _lastLog = string.Empty;

        internal void AssignStations(World world)
        {
            foreach (Bot bot in world.Bots)
            {
                if (bot.Station != null && !IsStillStation(world, bot.Station))
                {
                    bot.Station = null;
                }
            }

            foreach (Facility facility in world.Facilities)
            {
                if (!facility.IsStationed(world.InCombat) || world.StationedAt(facility) != null)
                {
                    continue;
                }

                Place post = Place.At(facility);
                Bot chosen = world.Bots.FirstOrDefault(bot => bot.Station == null && bot.Action.Holds(post, PlaceUse.Post))
                    ?? world.Bots.Where(bot => bot.Station == null && bot.IsIdle)
                        .OrderBy(bot => world.Paths.PathCost(bot.Position, facility.Door))
                        .FirstOrDefault();
                if (chosen != null)
                {
                    chosen.Station = facility;
                }
            }
        }

        private static bool IsStillStation(World world, Facility station)
        {
            return station.IsStationed(world.InCombat) && world.Facilities.Find(station.ID) == station;
        }

        internal void Assign(World world)
        {
            foreach (Bot bot in world.Bots.Where(b => b.IsIdle))
            {
                bot.IsLost = false;
            }

            var assignments = new WorldAssignment(world);
            assignments.CollectAll(_actions);
            int passed = 0;
            for (int n = 0; n < MaxAssignments && passed < _actions.Count; n++)
            {
                AssignmentAction action = _actions[passed];
                if (!action.IsReachable(world))
                {
                    passed++;
                    continue;
                }

                if (TryStartNearest(world, action))
                {
                    assignments.CollectAll(_actions);
                    continue;
                }

                if (!world.Bots.Any(bot => bot.IsIdle))
                {
                    break;
                }

                passed++;
            }

            LogUnassigned(world, _actions.Skip(passed));
        }

        private bool TryStartNearest(World world, AssignmentAction action)
        {
            _tried.Clear();
            for (Bot bot = NearestFree(world, action); bot != null; bot = NearestFree(world, action))
            {
                if (bot.TryStart(world, action))
                {
                    return true;
                }

                _tried.Add(bot);
            }

            return false;
        }

        private Bot NearestFree(World world, AssignmentAction action)
        {
            return world.Bots.Where(bot => bot.IsIdle && !_tried.Contains(bot) && bot.MayTake(world, action))
                .OrderBy(bot => world.Paths.PathCost(bot.Position, action.SpotFor(world, bot)))
                .FirstOrDefault();
        }

        private void LogUnassigned(World world, IEnumerable<AssignmentAction> unassigned)
        {
            if (!world.Log.IsEnabled)
            {
                return;
            }

            string text = string.Join("、", unassigned.Select(left => left.LogText(world)));
            if (text == _lastLog)
            {
                return;
            }

            _lastLog = text;
            string line = text.Length == 0 ? "未割当のアクションなし" : "未割当のアクション: " + text;
            world.Log.Write(() => line);
        }
    }
}
