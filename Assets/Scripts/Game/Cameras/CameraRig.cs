using Scrapwalker.Game.Look;
using Scrapwalker.Game.MainView;
using Scrapwalker.Game.MainView.Outpost.Limbs;
using Scrapwalker.Game.UI;
using UnityEngine;

namespace Scrapwalker.Game.Cameras
{
    public sealed class CameraRig
    {
        private const float BaseSpan = 940f;
        private const float BaseHeight = 360f;
        private const float MaxZoom = 1.95f;
        private const float GroundGap = 46f;
        private const float HeightMargin = 40f;
        private const float ScrollMin = -30f;
        private const float SpawnMargin = 40f;
        private const float EdgeBand = 0.25f;
        private const float ScrollPixelsPerSecond = 540f;
        private const float ScrollLag = 0.78f;
        private const float ScrollSnapPixels = 0.3f;
        private const float SettleSeconds = 0.35f;

        private readonly Camera _camera;

        private float _scroll;
        private float _scrollTo;
        private float _scrollMax;
        private float _enemiesWeight;
        private float _scrollFrom;

        private bool _hasLast;
        private bool _lastMoving;
        private Vector2 _lastOffset;
        private Vector2 _lastLayout;
        private Vector2 _jump;
        private Vector2 _jumpVelocity;
        private float _height = UILayout.ReferenceHeight;

        public CameraRig(Camera camera)
        {
            _camera = camera;
            _camera.orthographic = true;
            _camera.backgroundColor = Palette.Background;
            _camera.clearFlags = CameraClearFlags.SolidColor;
            _camera.rect = new Rect(0f, 0f, 1f, 1f);
            Place(new Vector2(WorldScene.ScreenWidth * 0.5f, WorldScene.ScreenHeight * 0.5f), 1f);
        }

        public bool IsScrollable { get; private set; }
        public bool CanScrollLeft => _scrollTo > ScrollMin + 0.5f;
        public bool CanScrollRight => _scrollTo < _scrollMax - 0.5f;
        public float PullLeft { get; private set; }
        public float PullRight { get; private set; }

        public Rect ViewPixels { get; private set; }

        public void Tick(in CameraFrame frame, in CameraFocus focus)
        {
            float deltaTime = Mathf.Min(frame.DeltaSeconds, 1f / 30f);
            var screen = new Vector2(Mathf.Max(1f, Screen.width), Mathf.Max(1f, Screen.height));
            Vector2 panel = frame.PanelSize ?? new Vector2(UILayout.ReferenceHeight * screen.x / screen.y, UILayout.ReferenceHeight);
            float referenceWidth = panel.x;
            _height = panel.y;
            float top = UILayout.TopHeight;
            float bottom = UILayout.BottomHeight;
            float zoom = Mathf.Min(MaxZoom, Mathf.Min(referenceWidth / BaseSpan, (_height - top - bottom - HeightMargin) / BaseHeight));

            float viewWidth = referenceWidth / zoom;
            _scrollMax = Mathf.Max(ScrollMin, WorldScene.EnemySpawnX - SpawnMargin - viewWidth);
            IsScrollable = frame.IsScrollable && !frame.Cue.IsMoving;
            EdgeScroll(frame, screen, top, bottom, Mathf.Min(frame.RealDeltaSeconds, 1f / 30f));
            ShowEnemies(focus, viewWidth);
            Vector2 offset = Settle(frame, OffsetOf(frame, referenceWidth, zoom, bottom), panel, deltaTime) + frame.Cue.Shake;
            Vector2 center = (panel * 0.5f - offset) / zoom;
            float facilityWeight = focus.FacilityWeight * (1f - frame.Aside);
            if (facilityWeight > 0f)
            {
                FocusFacility(focus, facilityWeight, top, ref center, ref zoom);
            }

            Place(center, zoom);
            Vector2 size = panel / zoom;
            ViewPixels = new Rect(center - size * 0.5f, size);
        }

        public void Snap()
        {
            _hasLast = false;
            _jump = Vector2.zero;
            _jumpVelocity = Vector2.zero;
        }

        private void EdgeScroll(in CameraFrame frame, Vector2 screen, float top, float bottom, float deltaTime)
        {
            PullLeft = 0f;
            PullRight = 0f;
            if (frame.Cue.IsMoving)
            {
                _scroll = _scrollTo = 0f;
                return;
            }

            if (frame.Cue.IsLaunching)
            {
                _scrollTo = 0f;
            }

            if (frame.IsScrollable)
            {
                float across = frame.Cursor.x / screen.x;
                float down = (1f - frame.Cursor.y / screen.y) * _height;
                if (across >= 0f && across <= 1f && down >= top && down <= _height - bottom)
                {
                    PullRight = Mathf.Max(0f, across - (1f - EdgeBand)) / EdgeBand;
                    PullLeft = Mathf.Max(0f, EdgeBand - across) / EdgeBand;
                    _scrollTo += (PullRight - PullLeft) * ScrollPixelsPerSecond * deltaTime;
                }
            }

            _scrollTo = Mathf.Clamp(_scrollTo, ScrollMin, _scrollMax);
            _scroll += (_scrollTo - _scroll) * (1f - Mathf.Pow(ScrollLag, deltaTime * WorldScene.FramesPerSecond));
            if (Mathf.Abs(_scrollTo - _scroll) < ScrollSnapPixels)
            {
                _scroll = _scrollTo;
            }
        }

        private void ShowEnemies(in CameraFocus focus, float viewWidthPixels)
        {
            if (focus.EnemiesWeight <= 0f)
            {
                _enemiesWeight = 0f;
                return;
            }

            if (_enemiesWeight <= 0f)
            {
                _scrollFrom = _scroll;
            }
            else if (focus.EnemiesWeight < _enemiesWeight)
            {
                _scrollFrom = 0f;
            }

            _enemiesWeight = focus.EnemiesWeight;
            float target = Mathf.Clamp(focus.EnemiesPixels - viewWidthPixels * 0.5f, ScrollMin, _scrollMax);
            _scroll = _scrollTo = Mathf.Lerp(_scrollFrom, target, focus.EnemiesWeight);
        }

        private Vector2 OffsetOf(in CameraFrame frame, float referenceWidth, float zoom, float bottom)
        {
            float groundY = _height - bottom - GroundGap;
            float rise = frame.Cue.IsMoving ? OutpostMotion.RiseHeight * frame.Cue.RiseProgress : 0f;
            float x = frame.Cue.IsMoving ? referenceWidth / 2f - zoom * frame.Cue.BaseCenterPixels : -(_scroll + frame.Cue.Pan.x) * zoom;
            float outpostX = x + zoom * frame.Cue.BaseCenterPixels;
            x += frame.Aside * Mathf.Max(0f, referenceWidth + zoom * BaseSpan * 0.5f - outpostX);
            return new Vector2(x, groundY - zoom * WorldScene.Ground + zoom * (rise + frame.Cue.Pan.y));
        }

        private Vector2 Settle(in CameraFrame frame, Vector2 offset, Vector2 layout, float deltaTime)
        {
            if (_hasLast && (frame.Cue.IsMoving != _lastMoving || layout != _lastLayout))
            {
                _jump += _lastOffset - offset;
            }

            _hasLast = true;
            _lastMoving = frame.Cue.IsMoving;
            _lastLayout = layout;
            _lastOffset = offset;
            _jump.x = Mathf.SmoothDamp(_jump.x, 0f, ref _jumpVelocity.x, SettleSeconds, float.PositiveInfinity, deltaTime);
            _jump.y = Mathf.SmoothDamp(_jump.y, 0f, ref _jumpVelocity.y, SettleSeconds, float.PositiveInfinity, deltaTime);
            return offset + _jump;
        }

        private void FocusFacility(in CameraFocus focus, float facilityWeight, float bandTop, ref Vector2 center, ref float zoom)
        {
            float bandBottom = UILayout.LowTalkTop;
            float focusZoom = Mathf.Max(zoom, (bandBottom - bandTop) / (focus.FacilityHeightPixels + WorldScene.CellSize));
            var focusCenter = new Vector2(focus.FacilityPixels.x, focus.FacilityPixels.y + (_height * 0.5f - (bandTop + bandBottom) * 0.5f) / focusZoom);
            zoom = Mathf.Lerp(zoom, focusZoom, facilityWeight);
            center = Vector2.Lerp(center, focusCenter, facilityWeight);
        }

        private void Place(Vector2 center, float zoom)
        {
            _camera.orthographicSize = _height / zoom / WorldScene.CellSize * 0.5f;
            Vector2 units = WorldScene.UnitsOfPixels(center);
            _camera.transform.position = new Vector3(units.x, units.y, -10f);
        }
    }
}
