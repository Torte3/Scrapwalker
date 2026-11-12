using Scrapwalker.Core.Framework;
using Scrapwalker.Core.Outpost;
using Scrapwalker.Core.Outpost.Grid;
using Scrapwalker.Core.Root;
using Scrapwalker.Core.Save;
using Scrapwalker.Core.Sessions;
using Scrapwalker.Core.Sessions.Tutorial;
using Scrapwalker.Core.Travel;
using Scrapwalker.Game.Audio;
using Scrapwalker.Game.Cameras;
using Scrapwalker.Game.Definitions;
using Scrapwalker.Game.Input;
using Scrapwalker.Game.Look;
using Scrapwalker.Game.MainView;
using Scrapwalker.Game.UI;
using Scrapwalker.Game.UI.Build;
using Scrapwalker.Game.UI.Screens.Tutorial;
using Scrapwalker.Game.Wording;
using UnityEngine;

namespace Scrapwalker.Game.App
{
    public sealed class GameController : MonoBehaviour, IAppHost
    {
        private const float MaxFrameSeconds = 0.1f;

        [SerializeField] private GameData _data;
        [SerializeField] private HUDView _hud;
        [SerializeField] private WorldView _worldView;
        [SerializeField] private Camera _camera;
        [SerializeField] private bool _verboseLog = true;
        private CameraRig _rig;
        private GameInput _input;
        private readonly BoolEdge _dark = new BoolEdge();
        private readonly TitlePan _titlePan = new TitlePan();
        private readonly GameEventListeners _listeners = new GameEventListeners();

        public Session Session { get; private set; }
        public UIState UI { get; private set; }
        public EventLog Log { get; private set; }
        public TutorialGuide TutorialGuide { get; private set; }
        public GameAudio Audio { get; private set; }
        public bool IsCinematic => Session.World.Takeoff.IsActive || _titlePan.IsSliding;
        public TutorialCue Cue { get; private set; }
        public bool IsExplaining { get; private set; }
        public float GameDelta { get; private set; }
        public GridOnScreen GridOnScreen { get; private set; }
        IGridOnScreen IAppHost.GridOnScreen => GridOnScreen;
        public VolumeSettings Volumes => Audio.Volumes;
        public float UIDelta { get; private set; }

        private void Awake()
        {
            Session = new Session(_data.Definitions, _data.Tutorial, TutorialScript.Beats, Application.persistentDataPath, line => line.Wording(_data), _data.NameOf,
                _verboseLog ? message => Debug.Log("Scrapwalker " + message) : (System.Action<string>)null,
                message => Debug.LogWarning("Scrapwalker " + message),
                message => Debug.LogError("Scrapwalker " + message));
            _rig = new CameraRig(_camera);
            CreateParts();
            Session.WorldReplaced += Session_WorldReplaced;
            Session.ReplaceWithNewGame();
            OpenTitle();
        }

        // Mixer は Awake の間は音量の書き換えを受け付けない
        private void Start()
        {
            Audio.Volumes.ApplyAll();
        }

        public void ShowTitle()
        {
            Session.ReplaceWithNewGame();
            OpenTitle();
        }

        private void OpenTitle()
        {
            _hud.OpenTitle();
            _titlePan.Hold();
            _worldView.ShowTimeOfDay(Session.Saves.Describe(SaveStore.AutoSlot).TimeOfDay);
        }

        public void SlideInFromTitle()
        {
            _titlePan.SlideIn();
            _worldView.HoldDeparture();
        }

        public void Quit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        private void CreateParts()
        {
            UI = new UIState(new BuildTool(Session, _data));
            GridOnScreen = new GridOnScreen(_camera, _worldView);
            _input = new GameInput(Session, UI, GridOnScreen);
            Log = new EventLog(_hud.Ticker, _data);
            TutorialGuide = new TutorialGuide(Session, UI, _hud.TutorialTargets);
            Audio = new GameAudio(transform, _data.Sounds, _data);
            _hud.ButtonClicked += Audio.OnButtonClicked;
            _hud.WindowOpened += () => Audio.Request(UISound.WindowOpen);
            _hud.WindowClosed += () => Audio.Request(UISound.WindowClose);
            UI.Build.Ran += () => Audio.Request(UISound.Placed);
            _input.LockedOn += () => Audio.Request(UISound.LockOn);
            Session.CommandRejected += _ => Audio.Request(UISound.Rejected);
            Listen();
        }

        private void Session_WorldReplaced()
        {
            ShowWorld();
        }

        private void ShowWorld()
        {
            World world = Session.World;
            _worldView.Initialize(world, _data);
            _rig.Snap();
            if (Session.Blackout.IsActive)
            {
                _worldView.ShowTimeOfDay(world.TimeOfDay);
            }

            UI.Reset();
            _titlePan.Clear();
            Audio.OnWorldReplaced(world);
            _hud.Bind(new UIContext(Session, UI, this, _data));
        }

        public void PlaySound(UISound sound)
        {
            Audio.Request(sound);
        }

        private void Update()
        {
            float realDeltaTime = UnityEngine.Time.deltaTime;
            float frameSeconds = Mathf.Min(realDeltaTime, MaxFrameSeconds);
            UIDelta = frameSeconds;
            if (_hud.IsTitleOpen)
            {
                UpdateTitle(realDeltaTime, frameSeconds);
                return;
            }

            if (_hud.IsSaveScreenOpen)
            {
                GameDelta = 0f;
                RefreshScreen();
                return;
            }

            StepGame(realDeltaTime, frameSeconds);
            RenderWorld(PlayInput(frameSeconds));
            ResolveCueForScreen();
            RefreshScreen();
        }

        private void UpdateTitle(float realDeltaTime, float frameSeconds)
        {
            GameDelta = 0f;
            Session.Advance(realDeltaTime, true);
            RenderWorld(TitleInput(frameSeconds));
            RefreshScreen();
        }

        private WorldInput PlayInput(float frameSeconds)
        {
            return new WorldInput
            {
                GameDelta = GameDelta,
                TickShare = Session.Time.TickShare,
                RealDelta = frameSeconds,
                Selected = UI.Selected,
                Marked = UI.Build.Relocating,
                Ghost = UI.Build.Ghost,
                AIWake = Session.Tutorial.WakeProgress,
            };
        }

        private WorldInput TitleInput(float frameSeconds)
        {
            return new WorldInput { RealDelta = frameSeconds, TickShare = Session.Time.TickShare, HoldsTimeOfDay = true, Marked = UI.Build.Relocating, AIWake = 1f };
        }

        private void StepGame(float realDeltaTime, float frameSeconds)
        {
            if (!_titlePan.IsSliding)
            {
                _worldView.ReleaseDeparture();
            }

            ResolveCue();
            Session.Advance(realDeltaTime, UI.StopsTime || IsExplaining || _worldView.HoldsDeparture);
            GameDelta = frameSeconds * Session.Time.Scale;

            _input.Handle(!IsCinematic);
            Session.Tutorial.Tick(UIDelta, GameDelta, UI.IsIdle, _worldView);
            foreach (GameEvent gameEvent in Session.DrainEvents())
            {
                _listeners.Dispatch(gameEvent);
            }

            Session.TickTalk(UIDelta, GameDelta);

            Log.Flush();
            TickBlackout();
        }

        private void RenderWorld(in WorldInput input)
        {
            _worldView.Step(input);
            TickCamera();
            _worldView.Draw(_rig.ViewPixels);
        }

        private void RefreshScreen()
        {
            _hud.Refresh();
            Audio.Tick(new AudioFrame
            {
                UIDelta = UIDelta,
                GameDelta = GameDelta,
                IsTitle = _hud.IsTitleOpen,
                World = Session.World,
                WakeProgress = Session.Tutorial.WakeProgress,
                Speaker = Session.Speaker,
                Scene = _worldView.Cues,
            });
        }

        private void ResolveCueForScreen()
        {
            ResolveCue();
        }

        private void ResolveCue()
        {
            TutorialFlow tutorial = Session.Tutorial;
            Cue = tutorial.IsActive ? TutorialGuide.Cue() : default;
            IsExplaining = Cue.Explanation != null && !Session.Speaker.ShowsTalk && !tutorial.IsBlacking;
        }

        private void Listen()
        {
            Audio.Listen(_listeners);
            _hud.WaveBanner.Listen(_listeners);
            UI.Listen(_listeners);
            _worldView.Listen(_listeners);
            Log.Listen(_listeners);
            _listeners.On<TakeoffFinished>(_ => _hud.OpenClearScreen());
            _listeners.On<Restarted>(_ => ShowWorld());
        }

        private void TickBlackout()
        {
            if (_dark.Rose(Session.Blackout.Step.IsDark))
            {
                _worldView.ClearEffects();
            }
        }

        private void TickCamera()
        {
            bool scrollable = !_input.IsOverUI && Session.World.InCombat && !IsCinematic;
            var frame = new CameraFrame(_worldView.CameraCue, scrollable, _input.Pointer, GameDelta, UIDelta, _titlePan.Aside, _hud.PanelSize);
            _titlePan.Tick(UIDelta);
            _rig.Tick(frame, TutorialFocus());
            _hud.EdgeHint.Set(_rig.IsScrollable && _rig.CanScrollLeft, _rig.IsScrollable && _rig.CanScrollRight, _rig.PullLeft, _rig.PullRight);
        }

        private CameraFocus TutorialFocus()
        {
            TutorialFlow tutorial = Session.Tutorial;
            Facility facility = tutorial.FocusFacility;
            CellRect rect = facility != null ? facility.Rect : default;
            Vector2 facilityPixels = facility != null ? _worldView.PixelsOfCell(rect.Column + rect.Width * 0.5f, rect.Row + rect.Height * 0.5f) : Vector2.zero;
            float facilityHeightPixels = rect.Height * WorldScene.CellSize;
            float facilityWeight = facility != null ? tutorial.FocusWeight() : 0f;
            return new CameraFocus(facilityPixels, facilityHeightPixels, facilityWeight, _worldView.EnemiesCenterPixels, tutorial.EnemiesWeight());
        }

        private void OnDestroy()
        {
            _input?.Dispose();
        }
    }
}
