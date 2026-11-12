using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Scrapwalker.Core.Outpost.Facilities;
using Scrapwalker.Core.Outpost.Grid;
using Scrapwalker.Core.Outpost.Roles;

namespace Scrapwalker.Core.Outpost
{
    public sealed class FacilityList : IReadOnlyList<Facility>
    {
        private readonly List<Facility> _facilities = new List<Facility>();

        public int Count => _facilities.Count;
        public Facility this[int index] => _facilities[index];

        public IEnumerator<Facility> GetEnumerator() => _facilities.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        internal void Add(Facility facility)
        {
            _facilities.Add(facility);
        }

        internal void Remove(Facility facility)
        {
            _facilities.Remove(facility);
        }

        public Facility Find(int id)
        {
            return _facilities.Find(f => f.ID == id);
        }

        public Facility Find(Place place)
        {
            return _facilities.Find(place.IsAt);
        }

        public Facility At(Cell cell)
        {
            return _facilities.Find(f => f.Covers(cell));
        }

        public WorkTask FindTask(System.Predicate<RecipeDefinition> match, out Facility facility)
        {
            foreach (Facility f in _facilities)
            {
                WorkTask task = f.Work.Queue.Tasks.FirstOrDefault(o => match(o.Recipe));
                if (task != null)
                {
                    facility = f;
                    return task;
                }
            }

            facility = null;
            return null;
        }

        public Facility FirstOperational(FacilityDefinition definition)
        {
            return _facilities.Find(f => f.IsOperational && f.Definition == definition);
        }

        public int CountOperational(FacilityDefinition definition)
        {
            return _facilities.Count(f => f.IsOperational && f.Definition == definition);
        }

        public float SumOperational(FacilityStat stat)
        {
            return _facilities.Where(f => f.IsOperational).Sum(f => f.Stat(stat));
        }

        public int CountIncludingUnbuilt(FacilityDefinition definition)
        {
            return _facilities.Count(f => f.Definition == definition);
        }

        public Facility RelocatingInto(Cell cell)
        {
            return _facilities.Find(f => f.RelocateRect?.Covers(cell) == true);
        }
    }
}
