using Scrapwalker.Core.Combat;
using Scrapwalker.Core.Root;
using Scrapwalker.Core.Sessions.Tutorial;
using Scrapwalker.Core.Travel;
using Scrapwalker.Game.App;
using Scrapwalker.Game.Audio;
using Scrapwalker.Game.Cameras;
using Scrapwalker.Game.Definitions;
using Scrapwalker.Game.MainView.Backdrop;
using Scrapwalker.Game.MainView.Compose;
using Scrapwalker.Game.MainView.Effects;
using Scrapwalker.Game.MainView.Enemies;
using Scrapwalker.Game.MainView.Launch;
using Scrapwalker.Game.MainView.Outpost;
using Scrapwalker.Game.MainView.Outpost.Limbs;
using UnityEngine;

namespace Scrapwalker.Game.MainView
{
    public sealed class WorldView : MonoBehaviour, ISceneMotion
    {
        private readonly FrameVisual _frame = new FrameVisual();
        private readonly float[] _sceneSeconds = { -1f, -1f, -1f, -1f, -1f };
        private readonly SkyMix _sky = new SkyMix();
        private System.Func<TimeOfDay, Color> _motes;
        private World _world;
        private GameData _data;
        private WorldComposer _composer;
        private WorldInput _input;
        private Color _groundTint;
        private Vector2 _shake;
        private Jitter _jitter;
        private WorldScene _scene;
        private ArcMesh _arcs;
        private Transform _pixels;
        private SpriteBatch _sprites;
        private readonly DrawOrder _order = new DrawOrder();
        private WorldUI _ui;
        private WorldUI _labelUI;
        private OutpostMotion _motion;
        private BackdropPainter _backdrop;
        private RuinPainter _ruins;
        private EffectsPainter _effects;
        private EnemiesView _enemies;
        private LegPainter _legs;
        private ArmMotion _arm;
        private ArmPainter _armPainter;
        private OutpostView _outpost;
        private LaunchView _launch;

        public CameraCue CameraCue => new CameraCue(_world.Journey.IsTraveling || _scene.Rise > 0.01f, Mathf.Clamp01(_scene.Rise / OutpostMotion.RiseHeight),
            WorldScene.HullLeft + _scene.HullWidth * 0.5f + _scene.BaseX, _launch.Motion.IsActive,
            _launch.Pan, _shake);

        public SceneCues Cues => new SceneCues(_arm.IsReaching, _arm.IsCarrying, _arm.IsDigging, _motion.HasTouchedDown, _launch.Motion.IsActive && _launch.Motion.IsFired,
            IsAnyBot(activity => activity.SoundsWelding), IsAnyBot(activity => activity.SoundsRepairing));

        public float EnemiesCenterPixels => _enemies.Enemies.CenterPixels;

        public bool HoldsDeparture => _motion.IsHolding;

        private Vector2 BaseOffset => new Vector2(_scene.BaseX, _scene.BaseY);

        public Vector3 UnitsOfCell(float column, float row)
        {
            return WorldScene.UnitsOfPixels(PixelsOfCell(column, row));
        }

        public Vector2 PixelsOfCell(float column, float row)
        {
            return new Vector2(WorldScene.CellLeft(column), WorldScene.FloorY(row)) + BaseOffset;
        }

        public int EnemyAt(Vector2 pixels)
        {
            return _enemies.Enemies.EnemyAt(pixels);
        }

        public Vector2 CellOfUnits(Vector3 units)
        {
            return WorldScene.UnitsOfPixels(WorldScene.PixelsOfUnits(units) - BaseOffset);
        }

        public void Initialize(World world, GameData data)
        {
            if (_scene == null)
            {
                Build(data);
            }

            _world = world;
            _sky.MoveTo(world.TimeOfDay, SkyMix.LoadSeconds);
            SyncScene();
            _scene.WalkDistance = 0f;
            ReturnToRest();
            _effects.Clear();
            _enemies.Clear();
            _outpost.Clear();
            _launch.Clear();
            _motion.Stop();
            if (world.Journey.IsTraveling)
            {
                _arm.BeginStow();
                _motion.StartCruising();
                _enemies.Enemies.LeaveRuin();
            }
        }

        private void Build(GameData data)
        {
            _pixels = new GameObject("Px").transform;
            _pixels.SetParent(transform, false);
            _pixels.localScale = new Vector3(1f / WorldScene.CellSize, -1f / WorldScene.CellSize, 1f);
            _pixels.localPosition = WorldScene.UnitsOfPixels(Vector2.zero);
            _data = data;
            _composer = new WorldComposer(data);
            _motes = time => data.LookOf(time).Motes;
            var canvas = new CanvasStack();
            _arcs = NewChild<ArcMesh>("Arcs");
            _arcs.Initialize(canvas, data.Shaders.VertexColor);
            _jitter = new Jitter();
            _scene = new WorldScene(canvas, _jitter);
            _sprites = NewChild<SpriteBatch>("Sprites");
            _sprites.Initialize(canvas, _order, data.Shaders);
            _ui = NewUI("WorldUi", canvas, data);
            _labelUI = NewUI("LabelUi", canvas, data);
            var parts = new ArtParts(_sprites, data.Art, canvas);
            var effects = NewChild<ParticleEffects>("Particles");
            effects.Initialize(WorldScene.FramesPerSecond, data.Particles);
            _effects = new EffectsPainter(_scene, effects);
            _motion = new OutpostMotion(_scene, _effects);
            var player = new ArtPlayer(_scene, parts, _effects);
            _backdrop = new BackdropPainter(_scene, parts, data);
            _ruins = new RuinPainter(_scene, parts, data);
            _enemies = new EnemiesView(_scene, parts, player, _arcs, _effects, _ui, data);
            _legs = new LegPainter(_scene, _motion, _effects, parts);
            _arm = new ArmMotion(_scene, _motion, parts.Art.Arm);
            _armPainter = new ArmPainter(_scene, _arm, _effects, parts);
            _outpost = new OutpostView(_scene, parts, player, _effects, _ui, _labelUI, data);
            _launch = new LaunchView(_scene, parts, _effects, _arm);
        }

        private WorldUI NewUI(string name, CanvasStack canvas, GameData data)
        {
            var ui = NewChild<WorldUI>(name);
            ui.Initialize(canvas, _order, data.Art.Effects.White.First.Sprite, data.Shaders.Sprite, data.WorldFont);
            return ui;
        }

        private T NewChild<T>(string name) where T : Component
        {
            var child = new GameObject(name).AddComponent<T>();
            child.transform.SetParent(_pixels, false);
            return child;
        }

        private void SyncScene()
        {
            _scene.SetGrid(_world.Columns, _world.Rows, _world.Configuration.SpawnDistance);
        }

        private void ReturnToRest()
        {
            _motion.Reset();
            _legs.Reset();
            _enemies.Shots.Clear();
            _armPainter.Clear();
            _enemies.Enemies.ClearRuinDecoration();
        }

        public void HoldDeparture()
        {
            if (_motion.IsMoving)
            {
                _motion.Hold();
            }
        }

        public void ReleaseDeparture()
        {
            _motion.Release();
        }

        public void ShowTimeOfDay(TimeOfDay time)
        {
            _sky.Snap(time);
        }

        public float SecondsIn(SceneMoment moment)
        {
            return _sceneSeconds[(int)moment];
        }

        private void TrackScene(bool isMoving)
        {
            Track(SceneMoment.Walking, _motion.HasStoodUp);
            Track(SceneMoment.PastRuin, HasPassedRuin(isMoving));
            Track(SceneMoment.Grounded, !isMoving && !_motion.IsLanding);
            Track(SceneMoment.Digging, _arm.IsDigging);
        }

        private bool HasPassedRuin(bool isMoving)
        {
            RuinDefinition left = _world.Journey.RuinBehind;
            return isMoving && _motion.HasStoodUp && (left == null || _scene.WorldX + _motion.LeftX + _ruins.RightEdge(left) <= 0f);
        }

        private void Track(SceneMoment moment, bool isNow)
        {
            int i = (int)moment;
            _sceneSeconds[i] = isNow ? Mathf.Max(0f, _sceneSeconds[i]) + _input.GameDelta : -1f;
        }

        private bool IsAnyBot(System.Func<BotActivity, bool> sounds)
        {
            foreach (BotVisual bot in _frame.Bots)
            {
                if (sounds(bot.Activity))
                {
                    return true;
                }
            }

            return false;
        }

        public void Listen(GameEventListeners listeners)
        {
            listeners.On<Landed>(_ => OnLanded());
            listeners.On<Departed>(_ => BeginWalk());
            listeners.On<DestinationChosen>(_ => BeginWalk());
            listeners.On<EnemyKilled>(killed => _enemies.Enemies.OnKilled(killed.EnemyID));
            listeners.On<ShotFired>(AddShot);
            listeners.On<ShotLanded>(OnShotLanded);
            listeners.On<EnemiesHealed>(healed => _enemies.Enemies.ShowHeal(healed));
            listeners.On<CellAttacked>(attacked => _enemies.Strike(attacked));
        }

        private void AddShot(ShotFired shot)
        {
            WeaponLook look = _data.Asset(shot.Weapon).Look.Weapon;
            _outpost.Facilities.Recoil(shot.FacilityID, look.RecoilFrames);
            _enemies.Shots.AddShot(shot, look.Shot);
        }

        private void OnShotLanded(ShotLanded landed)
        {
            _enemies.LandShot(landed, _data.Asset(landed.Weapon).Look.Weapon.Shot);
        }

        public void ClearEffects()
        {
            _effects.Clear();
        }

        private void OnLanded()
        {
            if (!_motion.BeginLanding())
            {
                ReturnToRest();
            }

            _effects.ClearRings();
            _enemies.Enemies.OnLanded();
        }

        private void BeginWalk()
        {
            if (_motion.IsMoving)
            {
                return;
            }

            _arm.BeginStow();
            _motion.Start();
            _legs.Reset();
            _enemies.Enemies.LeaveRuin();
            _enemies.Shots.Clear();
        }

        public void Step(in WorldInput input)
        {
            _input = input;
            SyncScene();
            _composer.Compose(_world, input, _frame);
            if (!input.HoldsTimeOfDay)
            {
                _sky.MoveTo(_world.TimeOfDay, SkyMix.PlaySeconds);
            }

            _sky.Step(input.RealDelta);
            _effects.TintMotes(_sky.Mix(_motes));
            _scene.Advance(input.GameDelta);
            float deltaFrames = _scene.StepFrames;
            _jitter.Frame(deltaFrames <= 0f);
            _launch.Booster.Step(_frame.Launch, deltaFrames);
            bool isMoving = StepMotion(deltaFrames);
            _launch.StepMotion(_frame.Launch, input.RealDelta * WorldScene.FramesPerSecond);
            _launch.Painter.Step(deltaFrames);
            _enemies.Step(_frame, deltaFrames, input.RealDelta);
            _outpost.Step(_frame, deltaFrames);
            _armPainter.Step(_frame.Arm, _world, ArmStowFrames(isMoving), deltaFrames);
            _shake = _scene.Shake > 0.5f ? new Vector2(_scene.Random() - 0.5f, _scene.Random() - 0.5f) * _scene.Shake : Vector2.zero;
            TrackScene(isMoving);
        }

        public void Draw(Rect view)
        {
            DrawScene(view, _motion.IsMoving, _input.Selected != null || _input.Ghost.HasValue);
            _effects.Step();
            _scene.EndFrame();
        }

        private float? ArmStowFrames(bool isMoving)
        {
            if (isMoving)
            {
                return _motion.FramesSinceStart;
            }

            return _launch.Motion.IsActive && _launch.Motion.FramesSinceLaunch >= 0f ? _launch.Motion.FramesSinceLaunch : (float?)null;
        }

        private bool StepMotion(float deltaFrames)
        {
            TravelVisual travel = _frame.Travel;
            bool isMoving = _world.Journey.IsTraveling;
            if (!isMoving && !_motion.IsLanding && _world.InCombat)
            {
                _motion.KeepRuinBehind();
            }

            if (isMoving)
            {
                BeginWalk();
                _motion.StepMove(deltaFrames, travel, _motion.IsHolding ? float.PositiveInfinity : _launch.BoosterMotion.StowFramesLeft);
                return true;
            }

            if (_motion.IsMoving)
            {
                _motion.Stop();
                if (!_motion.IsLanding)
                {
                    ReturnToRest();
                }
            }

            if (_motion.IsLanding)
            {
                if (_motion.StepLanding(deltaFrames))
                {
                    _enemies.Shots.Clear();
                }
            }
            else if (_motion.IsUnfolding)
            {
                _motion.StepUnfold(deltaFrames);
            }

            return false;
        }

        private void DrawScene(Rect view, bool isMoving, bool showsQuality)
        {
            _scene.Canvas.Begin();
            _order.Begin();
            _arcs.Begin(_order.Next(DrawBand.Effects));
            _sprites.Begin();
            _ui.Begin();
            _labelUI.Begin();
            DrawBehindOutpost(view, isMoving);
            DrawOutpost(isMoving, showsQuality);
            if (!isMoving)
            {
                DrawRuins((ruin, x) => _ruins.DrawFront(ruin, x, DrawBand.Arm, _groundTint));
            }

            InOutpost(_armPainter.DrawFront);
            _labelUI.End();

            _enemies.Draw(_frame);
            _effects.OrderEffects(_order);
            InOutpost(_launch.Painter.DrawFire);
            _outpost.Build.Draw(_frame);
            _launch.Painter.DrawFlash();
            _arcs.End();
            _sprites.End();
            _ui.End();
        }

        private void DrawBehindOutpost(Rect view, bool isMoving)
        {
            _backdrop.DrawSky(view, _sky);
            _launch.DrawSpace(view);
            _backdrop.DrawFar(view, _sky);
            _effects.OrderMotes(_order);
            _groundTint = _backdrop.GroundTint(_sky);
            _enemies.Enemies.DrawDecoration();
            DrawRuins((ruin, x) => _ruins.DrawBack(ruin, x, _groundTint));
            if (isMoving)
            {
                DrawRuins((ruin, x) => _ruins.DrawFront(ruin, x, DrawBand.Ruins, _groundTint));
            }

            _backdrop.DrawGround(view, _sky);
            _enemies.Enemies.DrawFading();
        }

        private void DrawOutpost(bool isMoving, bool showsQuality)
        {
            CanvasStack canvas = _scene.Canvas;
            canvas.Save();
            _scene.ApplyTilt();
            _legs.Draw(isMoving);

            canvas.Save();
            canvas.Translate(_scene.BaseX, _scene.BaseY);
            _launch.DrawBooster();
            _outpost.Hull.Draw(_frame.RadarLook);
            _outpost.DrawFacilities(_frame, showsQuality);
            _armPainter.Draw();
            _outpost.Bots.DrawNames();
            _outpost.Facilities.EmitParticles(_frame.Facilities);
            canvas.Restore();
            canvas.Restore();
        }

        private void InOutpost(System.Action draw)
        {
            CanvasStack canvas = _scene.Canvas;
            canvas.Save();
            _scene.ApplyTilt();
            canvas.Translate(_scene.BaseX, _scene.BaseY);
            draw();
            canvas.Restore();
        }

        private void DrawRuins(System.Action<RuinDefinition, float> draw)
        {
            Journey journey = _world.Journey;
            if (journey.RuinBehind != null && (_motion.IsMoving || _motion.IsLanding))
            {
                draw(journey.RuinBehind, _scene.WorldX + _motion.LeftX);
            }

            if (journey.RuinAhead != null)
            {
                draw(journey.RuinAhead, _scene.WorldX + _motion.AheadX);
            }
        }
    }
}
