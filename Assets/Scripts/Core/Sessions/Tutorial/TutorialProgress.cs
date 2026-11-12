using System.Collections.Generic;
using Scrapwalker.Core.Outpost;
using Scrapwalker.Core.Root;

namespace Scrapwalker.Core.Sessions.Tutorial
{
    public sealed class TutorialProgress
    {
        private readonly World _world;
        private readonly IReadOnlyList<TutorialBeat> _script;

        internal TutorialProgress(World world, IReadOnlyList<TutorialBeat> script)
        {
            _world = world;
            _script = script;
            At = End;
            world.Events.On<Restarted>(_ => Rewind());
        }

        public int End => _script.Count;

        public int At { get; private set; }

        public bool IsActive => At < End;

        public TutorialBeat Current => IsActive ? _script[At] : null;

        public bool IsCurrentDone => Current?.Done != null && Current.Done(_world);

        public TutorialGoal CurrentGoal => Current?.Goal ?? TutorialGoal.None;

        internal void Start()
        {
            GoTo(0);
        }

        internal bool TryAdvance()
        {
            if (!IsCurrentDone)
            {
                return false;
            }

            GoTo(At + 1);
            return true;
        }

        internal bool TryCompleteIntroduction()
        {
            return Current != null && Current.End.CompleteIntroduction(this);
        }

        internal void ReplaceWrecks()
        {
            for (int i = _world.Facilities.Count - 1; i >= 0; i--)
            {
                Facility f = _world.Facilities[i];
                if (f.Condition.IsRebuilding)
                {
                    FacilityCondition.Remove(_world, f);
                }
            }

            WorldFactory.AddStartMaterials(_world);
            GoTo(At + 1);
        }

        internal void Finish()
        {
            GoTo(End);
        }

        internal void GoTo(int at)
        {
            At = at;
            _world.Revision++;
            _world.CommandRule = IsActive ? CurrentGoal : CommandRule.All;
        }

        private void Rewind()
        {
            int at = At;
            while (at < End && at > 0)
            {
                TutorialBeat previous = _script[at - 1];
                if (previous.Done == null || previous.Done(_world))
                {
                    break;
                }

                at--;
            }

            GoTo(at);
        }
    }
}
