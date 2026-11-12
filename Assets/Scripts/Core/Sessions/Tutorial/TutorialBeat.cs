using System;
using System.Collections.Generic;
using Scrapwalker.Core.Items;
using Scrapwalker.Core.Root;
using Scrapwalker.Core.Travel;
using Scrapwalker.Core.Travel.Talk;

namespace Scrapwalker.Core.Sessions.Tutorial
{
    public sealed class TutorialBeat
    {
        private readonly List<TalkLine> _lines = new List<TalkLine>();
        private readonly List<TutorialView?> _views = new List<TutorialView?>();
        private readonly List<TutorialPrompt> _prompts = new List<TutorialPrompt>();
        private readonly List<TutorialExplanation> _explanations = new List<TutorialExplanation>();
        private readonly List<int> _explainAt = new List<int>();
        private int _foundRuinsAt = -1;

        public IReadOnlyList<TutorialExplanation> Explanations => _explanations;
        public TutorialGoal Goal { get; private set; } = TutorialGoal.None;
        public Func<World, bool> Done { get; private set; }
        public bool IsWait => Goal == TutorialGoal.None && Done != null;
        public string Hint { get; private set; }
        public bool IsWaking { get; private set; }
        public string Focus { get; private set; }
        public BeatEnd End { get; private set; } = BeatEnd.Await;
        public SceneMoment Cue { get; private set; }
        public float CueSeconds { get; private set; }

        public TutorialView FirstView => IsWaking ? TutorialView.Facility : TutorialView.Base;

        public TutorialBeat After(SceneMoment moment, float seconds = 0f)
        {
            Cue = moment;
            CueSeconds = seconds;
            return this;
        }

        public TutorialBeat WakeZoomedIn(string focus)
        {
            IsWaking = true;
            Focus = focus;
            return this;
        }

        public TutorialBeat ThenBlackout()
        {
            End = BeatEnd.Blackout;
            return this;
        }

        public TutorialBeat ThenFreePlay()
        {
            End = BeatEnd.FreePlay;
            return this;
        }

        public TutorialBeat Say(AIFace face, string text)
        {
            return Add(face, text, null);
        }

        public TutorialBeat Say(AIFace face, string text, TutorialView view)
        {
            return Add(face, text, view);
        }

        private TutorialBeat Add(AIFace face, string text, TutorialView? view)
        {
            _lines.Add(new TalkLine(face, text));
            _views.Add(view);
            return this;
        }

        public TutorialBeat SayFoundRuins()
        {
            _foundRuinsAt = _lines.Count;
            return this;
        }

        internal void LinesIn(World world, TutorialConfiguration configuration, List<SpokenLine> lines, List<TutorialView?> views, List<int> holds = null)
        {
            lines.Clear();
            views.Clear();
            holds?.Clear();
            int explanation = 0;
            for (int i = 0; i <= _lines.Count; i++)
            {
                if (i == _foundRuinsAt)
                {
                    AddFoundRuins(world, configuration, lines, views);
                }

                for (; explanation < _explainAt.Count && _explainAt[explanation] == i; explanation++)
                {
                    holds?.Add(lines.Count);
                }

                if (i < _lines.Count)
                {
                    lines.Add(new SpokenLine(_lines[i], null, 0));
                    views.Add(_views[i]);
                }
            }
        }

        private static void AddFoundRuins(World world, TutorialConfiguration configuration, List<SpokenLine> lines, List<TutorialView?> views)
        {
            foreach (RuinDefinition ruin in world.Exploration.OpenedBy(world.Definitions.Ruins, configuration.RadarRepair))
            {
                ruin.AddFoundLines(lines);
            }

            while (views.Count < lines.Count)
            {
                views.Add(null);
            }
        }

        public TutorialBeat Await(TutorialGoal goal)
        {
            Goal = goal;
            Done = goal.Reached;
            return this;
        }

        public TutorialBeat Point(TutorialStep step, string text, string material = null)
        {
            _prompts.Add(new TutorialPrompt { Step = step, Text = text, Material = material });
            return this;
        }

        public TutorialBeat Explain(TutorialStep? before, TutorialArea area, string text, string material = null)
        {
            _explanations.Add(new TutorialExplanation(before, area, text, material));
            _explainAt.Add(_lines.Count);
            return this;
        }

        public TutorialBeat ExplainOnOpen(TutorialArea area, string text)
        {
            return Explain(null, area, text);
        }

        public TutorialBeat WaitUntil(Func<World, bool> done)
        {
            Done = done;
            return this;
        }

        public TutorialBeat ShowWhileWaiting(string text)
        {
            Hint = text;
            return this;
        }

        public string PromptOf(TutorialStep step, MaterialDefinition material)
        {
            string any = null;
            foreach (TutorialPrompt prompt in _prompts)
            {
                if (prompt.Step != step)
                {
                    continue;
                }

                if (prompt.Material == null)
                {
                    any = any ?? prompt.Text;
                }
                else if (material != null && prompt.Material == material.Key)
                {
                    return prompt.Text;
                }
            }

            return any;
        }
    }
}
