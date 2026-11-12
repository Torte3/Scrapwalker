using Scrapwalker.Core.Framework;
using System;
using System.Collections.Generic;
using Scrapwalker.Core.Items;
using Scrapwalker.Core.Outpost;
using Scrapwalker.Core.Root;
using Scrapwalker.Core.Travel.Talk;

namespace Scrapwalker.Core.Sessions.Tutorial
{
    public sealed class TutorialFlow : ITalkSpeaker
    {
        private static readonly Phase Idle = new IdlePhase();
        private static readonly Phase Talking = new TalkingPhase();
        private static readonly Phase Waiting = new WaitingPhase();

        private readonly Func<Simulation> _simulation;
        private readonly TutorialConfiguration _configuration;
        private readonly TalkReader _talk;
        private readonly Blackout _blackout = new Blackout();
        private readonly List<SpokenLine> _lines = new List<SpokenLine>();
        private readonly List<TutorialView?> _views = new List<TutorialView?>();
        private readonly List<int> _holds = new List<int>();
        private readonly TimedPhase _waking = new TimedPhase(frame => frame.GameDeltaTime, flow => flow.Speak());
        private readonly TimedPhase _panning = new TimedPhase(frame => frame.UIDeltaTime, flow =>
        {
            flow.ViewFrom = flow.View;
            flow._phase = Talking;
        });
        private TutorialBeat _beat;
        private FacilityDefinition _focus;
        private Phase _phase = Idle;
        private int _explained;
        private int _resumeAt = -1;

        internal TutorialFlow(Func<Simulation> simulation, Func<SpokenLine, string> textOf, TutorialConfiguration configuration)
        {
            _simulation = simulation;
            _configuration = configuration;
            _talk = new TalkReader(textOf);
        }

        private World World => _simulation().World;
        private GameConfiguration Configuration => World.Configuration;

        public TutorialProgress Progress { get; private set; }
        public bool IsActive => Progress.IsActive && !World.IsDefeated;
        public bool IsTalking => IsActive && _talk.IsTalking;
        public bool IsPanning => IsActive && PanProgress < 1f;
        public float PanProgress => _phase == _panning ? _panning.Progress : 1f;
        public TutorialView View { get; private set; }
        public TutorialView ViewFrom { get; private set; }
        public float WakeProgress => IsActive && _phase == _waking ? _waking.Progress : 1f;
        public bool IsWaking => WakeProgress < 1f;
        public Facility FocusFacility => IsActive && (View == TutorialView.Facility || ViewFrom == TutorialView.Facility) && _focus != null
            ? World.Facilities.FirstOperational(_focus)
            : null;
        public bool IsBlacking => _blackout.IsActive;
        public bool ShowsTalk => IsActive && _phase.ShowsTalk;
        public bool PausesTime => IsActive && !IsWaking && (IsTalking || IsBlacking || IsPanning);
        public bool AllowsTime => IsActive && (_phase.AwaitsScene || _beat != null && _beat.IsWait);
        public bool LocksTime => IsActive && !AllowsTime;
        public bool IsWaiting => IsActive && _phase.IsWaiting;
        public string Hint => IsActive && _phase.IsWaiting ? _beat.Hint : null;

        public SpokenLine Line => _talk.Line;
        public int ShownChars => _talk.ShownChars;
        public bool IsLineShown => _talk.IsLineShown;

        public Blackout Blackout => _blackout;

        public string Prompt(TutorialStep step, MaterialDefinition material = null)
        {
            return _beat != null ? _beat.PromptOf(step, material) : null;
        }

        public bool TryGetExplanation(TutorialStep step, MaterialDefinition making, out TutorialExplanation explanation)
        {
            bool pending = IsActive && _phase.IsWaiting && _explained < _beat.Explanations.Count && _beat.Explanations[_explained].Precedes(step, making);
            explanation = pending ? _beat.Explanations[_explained] : default;
            return pending;
        }

        public void FinishExplanation()
        {
            _explained++;
            if (_resumeAt >= 0 && NextHold > _resumeAt)
            {
                TalkFrom(_resumeAt);
            }
        }

        private int NextHold => _explained < _holds.Count ? _holds[_explained] : int.MaxValue;

        internal void Begin(TutorialProgress progress)
        {
            Progress = progress;
            _blackout.Stop();
            Enter();
        }

        public void Tick(float uiDeltaTime, float gameDeltaTime, bool atRest, ISceneMotion scene)
        {
            if (!IsActive)
            {
                return;
            }

            if (_beat != Progress.Current)
            {
                Enter();
            }

            _phase.Tick(this, new Frame(uiDeltaTime, gameDeltaTime, atRest, scene));
        }

        public void Click()
        {
            if (!_phase.ShowsTalk)
            {
                return;
            }

            int line = _talk.LineIndex;
            _talk.Click();
            if (_talk.LineIndex != line)
            {
                TalkFrom(line + 1);
            }
        }

        private void Enter()
        {
            _beat = Progress.Current;
            _focus = _beat?.Focus != null ? World.Definitions.Named<FacilityDefinition>(_beat.Focus) : null;
            _explained = 0;
            _resumeAt = -1;
            _talk.Stop();
            View = ViewFrom = _beat?.FirstView ?? TutorialView.Base;
            if (_beat == null)
            {
                _phase = Idle;
            }
            else if (_beat.IsWaking)
            {
                _phase = _waking.Start(_configuration.WakeSeconds);
            }
            else if (_beat.Cue != SceneMoment.None)
            {
                _phase = new ScenePhase();
            }
            else
            {
                Speak();
            }
        }

        private void Speak()
        {
            _beat.LinesIn(World, _configuration, _lines, _views, _holds);
            TalkFrom(0);
        }

        private void TalkFrom(int line)
        {
            _resumeAt = -1;
            if (line < _lines.Count && NextHold <= line)
            {
                _talk.Stop();
                _resumeAt = line;
                AwaitGoal();
                return;
            }

            _talk.Start(_lines, line);
            if (_talk.IsTalking)
            {
                StartLine();
            }
            else
            {
                _beat.End.Begin(this);
            }
        }

        private void StartLine()
        {
            TutorialView? view = _views[_talk.LineIndex];
            if (view.HasValue && view.Value != View)
            {
                ViewFrom = View;
                View = view.Value;
                _phase = _panning.Start(_configuration.PanSeconds);
                return;
            }

            _phase = Talking;
        }

        internal void AwaitGoal()
        {
            _phase = Waiting;
        }

        internal void StartBlackout()
        {
            _blackout.Start(Configuration.BlackoutFadeSeconds, _configuration.HoldSeconds, Configuration.BlackoutFadeSeconds);
            _phase = new BlackoutPhase();
        }

        private readonly struct Frame
        {
            public Frame(float uiDeltaTime, float gameDeltaTime, bool atRest, ISceneMotion scene)
            {
                UIDeltaTime = uiDeltaTime;
                GameDeltaTime = gameDeltaTime;
                AtRest = atRest;
                Scene = scene;
            }

            public float UIDeltaTime { get; }
            public float GameDeltaTime { get; }
            public bool AtRest { get; }
            public ISceneMotion Scene { get; }
        }

        private abstract class Phase
        {
            public virtual bool AwaitsScene => false;
            public virtual bool ShowsTalk => false;
            public virtual bool IsWaiting => false;

            public abstract void Tick(TutorialFlow flow, in Frame frame);
        }

        private sealed class IdlePhase : Phase
        {
            public override void Tick(TutorialFlow flow, in Frame frame)
            {
            }
        }

        private sealed class TimedPhase : Phase
        {
            private readonly SecondsCountdown _time = new SecondsCountdown();
            private readonly Func<Frame, float> _clock;
            private readonly Action<TutorialFlow> _then;

            public TimedPhase(Func<Frame, float> clock, Action<TutorialFlow> then)
            {
                _clock = clock;
                _then = then;
            }

            public float Progress => _time.Progress;

            public TimedPhase Start(float seconds)
            {
                _time.Start(seconds);
                return this;
            }

            public override void Tick(TutorialFlow flow, in Frame frame)
            {
                if (_time.Advance(_clock(frame)))
                {
                    _then(flow);
                }
            }
        }

        private sealed class ScenePhase : Phase
        {
            public override bool AwaitsScene => true;

            public override void Tick(TutorialFlow flow, in Frame frame)
            {
                if (frame.Scene.SecondsIn(flow._beat.Cue) >= flow._beat.CueSeconds)
                {
                    flow.Speak();
                }
            }
        }

        private sealed class TalkingPhase : Phase
        {
            public override bool ShowsTalk => true;

            public override void Tick(TutorialFlow flow, in Frame frame)
            {
                flow._talk.Type(frame.UIDeltaTime, flow.Configuration.TalkCharsPerSecond);
            }
        }

        private sealed class WaitingPhase : Phase
        {
            public override bool IsWaiting => true;

            public override void Tick(TutorialFlow flow, in Frame frame)
            {
                if (flow._resumeAt < 0 && frame.AtRest && flow.Progress.TryAdvance())
                {
                    flow.Enter();
                }
            }
        }

        private sealed class BlackoutPhase : Phase
        {
            public override void Tick(TutorialFlow flow, in Frame frame)
            {
                flow._blackout.Advance(frame.UIDeltaTime, () =>
                {
                    flow.Progress.TryCompleteIntroduction();
                    flow._beat = flow.Progress.Current;
                });
                if (!flow._blackout.IsActive)
                {
                    flow.Enter();
                }
            }
        }
    }
}
