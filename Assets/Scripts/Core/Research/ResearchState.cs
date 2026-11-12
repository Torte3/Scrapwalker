using System.Collections.Generic;
using System.Linq;
using Scrapwalker.Core.Framework;
using Scrapwalker.Core.Items;
using Scrapwalker.Core.Outpost;
using Scrapwalker.Core.Outpost.Facilities;
using Scrapwalker.Core.Outpost.Facilities.Commands;
using Scrapwalker.Core.Root;

namespace Scrapwalker.Core.Research
{
    public sealed class ResearchState
    {
        private readonly World _world;
        private readonly HashSet<ResearchDefinition> _done = new HashSet<ResearchDefinition>();

        internal ResearchState(World world)
        {
            _world = world;
            world.Events.On<ResearchWorkFinished>(finished => Complete(finished.Research));
        }

        public ResearchDefinition Current => FindWork()?.Research;

        public ResearchWork FindWork()
        {
            foreach (ResearchDefinition research in _world.Definitions.Research)
            {
                RecipeDefinition recipe = _world.Definitions.RecipeFor(research);
                Facility facility = null;
                WorkTask task = recipe != null ? _world.Facilities.FindTask(r => r == recipe, out facility) : null;
                if (task != null)
                {
                    return new ResearchWork(research, task, facility);
                }
            }

            return null;
        }

        public bool IsWaitingForMaterials()
        {
            ResearchWork work = FindWork();
            return work != null && !work.Facility.Work.HasStarted(work.Task)
                && _world.Ledger.Shortage(work.Task.Recipe.Inputs, work.Task.Filter, work.DeliveredOf).Count > 0;
        }

        public bool IsShort(ResearchDefinition research)
        {
            return _world.Ledger.Shortage(_world.Definitions.RecipeFor(research).Inputs, QualityFilter.Any).Count > 0;
        }

        public bool IsDone(ResearchDefinition research)
        {
            return research == null || _done.Contains(research);
        }

        public bool ArePrerequisitesDone(ResearchDefinition research)
        {
            return research.Prerequisites.All(IsDone);
        }

        public ResearchStatus StatusOf(ResearchDefinition research)
        {
            if (IsDone(research))
            {
                return ResearchStatus.Done;
            }

            if (Current == research)
            {
                return ResearchStatus.Current;
            }

            return ArePrerequisitesDone(research) ? ResearchStatus.Available : ResearchStatus.Locked;
        }

        public Facility FacilityFor(ResearchDefinition research)
        {
            RecipeDefinition recipe = _world.Definitions.RecipeFor(research);
            if (recipe == null)
            {
                return null;
            }

            return _world.Facilities.FirstOrDefault(f => f.Definition.Recipes.Contains(recipe));
        }

        public AddWorkTask StartOf(ResearchDefinition research)
        {
            return new AddWorkTask(FacilityFor(research)?.ID ?? 0, _world.Definitions.RecipeFor(research)?.ID);
        }

        public RemoveWorkTask CancelOf()
        {
            ResearchWork work = FindWork();
            return work != null ? new RemoveWorkTask(work.Facility.ID, work.Task.ID) : null;
        }

        internal void Complete(ResearchDefinition research)
        {
            _done.Add(research);
            _world.Log.Write(() => "研究: 「" + research.Key + "」が済んだ");
        }

        internal ResearchMemento CreateMemento()
        {
            return new ResearchMemento { Done = _done.Select(research => research.ID).ToList() };
        }

        internal void Restore(ResearchMemento memento, LoadContext load)
        {
            _done.Clear();
            foreach (string id in memento.Done)
            {
                _done.Add(load.Find<ResearchDefinition>(id));
            }
        }
    }
}
