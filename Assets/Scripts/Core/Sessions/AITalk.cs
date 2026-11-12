using System.Collections.Generic;
using Scrapwalker.Core.Combat.Waves;
using Scrapwalker.Core.Framework;
using Scrapwalker.Core.Outpost;
using Scrapwalker.Core.Outpost.Upgrades;
using Scrapwalker.Core.Root;
using Scrapwalker.Core.Sessions.Tutorial;
using Scrapwalker.Core.Travel;
using Scrapwalker.Core.Travel.Talk;

namespace Scrapwalker.Core.Sessions
{
    public sealed class AITalk : ITalkSpeaker
    {
        private readonly Queue<PendingTalk> _queue = new Queue<PendingTalk>();
        private readonly TalkReader _reader;
        private readonly SecondsCountdown _wait = new SecondsCountdown();

        public AITalk(System.Func<SpokenLine, string> textOf)
        {
            _reader = new TalkReader(textOf);
        }

        public bool IsTalking => _reader.IsTalking;
        public bool ShowsTalk => IsTalking;
        public SpokenLine Line => _reader.Line;
        public int ShownChars => _reader.ShownChars;
        public bool IsLineShown => _reader.IsLineShown;

        public void Clear()
        {
            _queue.Clear();
            _reader.Stop();
        }

        internal void Listen(World world, TutorialProgress tutorial)
        {
            Hear<DestinationChosen>(world, tutorial, chosen => Enqueue(chosen.Ruin, TalkCue.Chosen));
            Hear<Landed>(world, tutorial, landed => Enqueue(landed.Ruin, TalkCue.Landed));
            Hear<WaveNearing>(world, tutorial, nearing => Enqueue(nearing.Ruin, TalkCue.WaveNearing));
            Hear<TookOff>(world, tutorial, tookOff => Enqueue(tookOff.From, tookOff.HasDefeatedAll ? TalkCue.DefeatedAll : TalkCue.EnemiesRetreated));
            Hear<RuinsInSightCleared>(world, tutorial, cleared =>
            {
                foreach (OutpostUpgradeDefinition upgrade in cleared.Upgrades)
                {
                    Enqueue(upgrade.Talks.TalkOn(TalkCue.AllOpenCleared), 0);
                }
            });
            Hear<DefeatBlackoutEnded>(world, tutorial, ended =>
            {
                if (ended.IsFirstDefeat)
                {
                    Enqueue(world.Configuration.Talks.TalkOn(TalkCue.FirstDefeat), 0);
                }
            });
            Hear<OutpostUpgraded>(world, tutorial, upgraded =>
            {
                Enqueue(upgraded.Upgrade.Talks.TalkOn(TalkCue.Upgraded), upgraded.Found);
                EnqueueFoundRuins(upgraded.Ruins);
            });
        }

        private static void Hear<T>(World world, TutorialProgress tutorial, System.Action<T> heard) where T : GameEvent
        {
            world.Events.On<T>(gameEvent =>
            {
                if (!tutorial.IsActive)
                {
                    heard(gameEvent);
                }
            });
        }

        public void Tick(float uiDeltaTime, float gameDeltaTime, World world)
        {
            if (world.IsDefeated)
            {
                Clear();
                return;
            }

            if (IsTalking)
            {
                _reader.Type(uiDeltaTime, world.Configuration.TalkCharsPerSecond);
                return;
            }

            if (_queue.Count == 0)
            {
                return;
            }

            if (!_wait.Advance(gameDeltaTime))
            {
                return;
            }

            _reader.Start(_queue.Dequeue().Lines);
            if (_queue.Count > 0)
            {
                _wait.Start(_queue.Peek().Delay);
            }
        }

        public void Click()
        {
            _reader.Click();
        }

        private void Enqueue(RuinDefinition ruin, TalkCue cue)
        {
            CuedTalk talk = ruin?.Talks.TalkOn(cue);
            if (talk != null)
            {
                Enqueue(new PendingTalk(talk.Delay, talk.Lines.ConvertAll(line => new SpokenLine(line, null, 0))));
            }
        }

        private void Enqueue(CuedTalk talk, int found)
        {
            if (talk != null)
            {
                Enqueue(new PendingTalk(talk.Delay, talk.Lines.ConvertAll(line => new SpokenLine(line, null, found))));
            }
        }

        private void EnqueueFoundRuins(IReadOnlyList<RuinDefinition> ruins)
        {
            var lines = new List<SpokenLine>();
            foreach (RuinDefinition ruin in ruins)
            {
                ruin.AddFoundLines(lines);
            }

            Enqueue(new PendingTalk(0f, lines));
        }

        private void Enqueue(PendingTalk talk)
        {
            if (talk.Lines.Count == 0)
            {
                return;
            }

            if (_queue.Count == 0)
            {
                _wait.Start(talk.Delay);
            }

            _queue.Enqueue(talk);
        }

        private sealed class PendingTalk
        {
            public PendingTalk(float delay, List<SpokenLine> lines)
            {
                Delay = delay;
                Lines = lines;
            }

            public float Delay { get; }
            public List<SpokenLine> Lines { get; }
        }
    }
}
