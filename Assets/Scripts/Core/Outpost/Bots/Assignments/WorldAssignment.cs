using System.Collections.Generic;
using Scrapwalker.Core.Outpost.Hulls;
using Scrapwalker.Core.Root;

namespace Scrapwalker.Core.Outpost.Bots.Assignments
{
    internal sealed class WorldAssignment
    {
        private static readonly int PriorityCount = System.Enum.GetValues(typeof(WorkPriority)).Length;

        private readonly World _world;
        private readonly List<Assignment> _children = new List<Assignment>();
        private readonly List<AssignmentAction>[] _byPriority = new List<AssignmentAction>[PriorityCount];

        public WorldAssignment(World world)
        {
            _world = world;
            for (int p = 0; p < _byPriority.Length; p++)
            {
                _byPriority[p] = new List<AssignmentAction>();
            }

            foreach (Facility facility in world.Facilities)
            {
                if (facility.Condition.HasSiteWork)
                {
                    _children.Add(new SiteAssignment(facility));
                }
            }

            foreach (Facility facility in world.Facilities)
            {
                if (facility.IsOperational && (facility.Work.Queue.Count > 0 || world.StationedAt(facility) != null))
                {
                    _children.Add(new FacilityAssignment(facility));
                }
            }

            foreach (Crack crack in world.Hull.Cracks)
            {
                _children.Add(new SiteAssignment(crack));
            }

            _children.Add(new FloorAssignment());
        }

        public void CollectAll(List<AssignmentAction> actions)
        {
            actions.Clear();
            foreach (List<AssignmentAction> bucket in _byPriority)
            {
                bucket.Clear();
            }

            foreach (Assignment child in _children)
            {
                foreach (AssignmentAction action in child.Actions(_world))
                {
                    _byPriority[(int)action.Priority].Add(action);
                }
            }

            for (int p = _byPriority.Length - 1; p >= 0; p--)
            {
                actions.AddRange(_byPriority[p]);
            }
        }
    }
}
