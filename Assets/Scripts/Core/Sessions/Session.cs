using System;
using System.Collections.Generic;
using Scrapwalker.Core.Framework;
using Scrapwalker.Core.Root;
using Scrapwalker.Core.Save;
using Scrapwalker.Core.Sessions.Tutorial;
using Scrapwalker.Core.Travel;
using Scrapwalker.Core.Travel.Talk;

namespace Scrapwalker.Core.Sessions
{
    public sealed class Session
    {
        private readonly GameDefinitions _definitions;
        private readonly TutorialConfiguration _tutorial;
        private readonly IReadOnlyList<TutorialBeat> _script;
        private readonly Action<string> _logSink;
        private readonly Blackout _newGameBlackout = new Blackout();
        private int _ticksSinceAutoSave;
        private SaveCues _saveCues;

        public Session(GameDefinitions definitions, TutorialConfiguration tutorial, IReadOnlyList<TutorialBeat> script, string saveRoot, Func<SpokenLine, string> talkText, Func<Definition, string> nameOf, Action<string> logSink, Action<string> warn, Action<string> error = null)
        {
            _definitions = definitions;
            _tutorial = tutorial;
            _script = script;
            _logSink = logSink;
            Saves = new SaveStore(saveRoot, definitions, nameOf, warn, error);
            Tutorial = new TutorialFlow(() => Simulation, talkText, tutorial);
            Talk = new AITalk(talkText);
        }

        internal Simulation Simulation { get; private set; }
        public World World => Simulation.World;
        public GameTime Time { get; } = new GameTime();
        public SaveStore Saves { get; }
        public TutorialFlow Tutorial { get; }
        public AITalk Talk { get; }
        public ITalkSpeaker Speaker => Tutorial.IsActive ? (ITalkSpeaker)Tutorial : Talk;

        public Blackout Blackout
        {
            get
            {
                if (_newGameBlackout.IsActive)
                {
                    return _newGameBlackout;
                }

                if (Tutorial.IsBlacking)
                {
                    return Tutorial.Blackout;
                }

                return World.DefeatBlackout;
            }
        }

        public event Action WorldReplaced;
        public event Action<CommandResult> CommandRejected;

        public void ReplaceWithNewGame()
        {
            Replace(NewWorld(), true);
        }

        public void FadeToNewGame()
        {
            _newGameBlackout.Start(World.Configuration.BlackoutFadeSeconds, 0f, World.Configuration.BlackoutFadeSeconds);
        }

        public bool Load(int slot = SaveStore.AutoSlot)
        {
            World world = Saves.Read(_definitions, slot);
            if (world == null)
            {
                return false;
            }

            Replace(world, false);
            return true;
        }

        public bool Save(int slot = SaveStore.AutoSlot)
        {
            if (!World.CanSave || !Saves.Write(SaveFile.Of(World), slot))
            {
                return false;
            }

            if (slot == SaveStore.AutoSlot)
            {
                _ticksSinceAutoSave = 0;
            }

            return true;
        }

        public bool SaveBeforeEscape(int slot)
        {
            SaveFile beforeEscape = _saveCues.BeforeEscape;
            return beforeEscape != null && Saves.Write(beforeEscape, slot);
        }

        public void SetSpeed(int speed)
        {
            Time.Speed = speed;
        }

        public void Advance(float deltaTime, bool screenPauses)
        {
            _newGameBlackout.Advance(deltaTime, ReplaceWithNewGame);
            if (Tutorial.LocksTime || World.Takeoff.IsActive || World.IsDefeated)
            {
                Time.Speed = 1;
            }

            Time.Advance(Simulation, deltaTime, FlowOf(screenPauses), AfterTick);
        }

        private TimeFlow FlowOf(bool screenPauses)
        {
            return screenPauses || Talk.IsTalking || Tutorial.PausesTime ? TimeFlow.Stopped : TimeFlow.Normal;
        }

        private void AfterTick()
        {
            WriteAutoSaveCue();
            CountAutoSave();
        }

        private void WriteAutoSaveCue()
        {
            SaveFile cue = _saveCues.TakeAutoSave();
            if (cue != null && Saves.Write(cue, SaveStore.AutoSlot))
            {
                _ticksSinceAutoSave = 0;
            }
        }

        private void CountAutoSave()
        {
            if (!World.CanSave)
            {
                return;
            }

            _ticksSinceAutoSave++;
            if (_ticksSinceAutoSave * World.Configuration.TickSeconds >= World.Configuration.AutoSaveSeconds)
            {
                Save();
            }
        }

        public CommandResult Execute(Command command)
        {
            CommandResult result = Simulation.Execute(command);
            WriteAutoSaveCue();
            if (!result.IsOK)
            {
                CommandRejected?.Invoke(result);
            }

            return result;
        }

        public CommandPreview Preview(Command command)
        {
            return Simulation.Preview(command);
        }

        public GameEvent[] DrainEvents()
        {
            return World.Events.Drain();
        }

        public void TickTalk(float uiDeltaTime, float gameDeltaTime)
        {
            Talk.Tick(uiDeltaTime, gameDeltaTime, World);
        }

        private World NewWorld()
        {
            return WorldFactory.CreateTutorial(_definitions, _tutorial);
        }

        private void Replace(World world, bool startsTutorial)
        {
            world.Log.Sink = _logSink;
            Simulation = new Simulation(world);
            _saveCues = new SaveCues(world);
            var tutorial = new TutorialProgress(world, _script);
            if (startsTutorial)
            {
                tutorial.Start();
            }

            _ticksSinceAutoSave = 0;
            Talk.Clear();
            Talk.Listen(world, tutorial);
            Tutorial.Begin(tutorial);
            WorldReplaced?.Invoke();
        }
    }
}
