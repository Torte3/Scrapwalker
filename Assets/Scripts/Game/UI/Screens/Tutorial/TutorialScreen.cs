using System;
using System.Collections.Generic;
using System.Linq;
using Scrapwalker.Core.Outpost;
using Scrapwalker.Core.Outpost.Grid;
using Scrapwalker.Core.Sessions;
using Scrapwalker.Core.Sessions.Tutorial;
using Scrapwalker.Core.Travel.Talk;
using Scrapwalker.Game.Look;
using Scrapwalker.Game.UI.Widgets;
using Scrapwalker.Game.Wording;
using Scrapwalker.Game.Wording.Words;
using UnityEngine;
using UnityEngine.UIElements;

namespace Scrapwalker.Game.UI.Screens.Tutorial
{
    public sealed class TutorialScreen : UIScreen
    {
        public const float HintWidth = 560f;

        private const float RingPadding = 6f;
        private const float Margin = 16f;
        private const float PulseSeconds = 1.4f;

        private enum TalkPlace
        {
            Middle,
            Low,
            At,
            Hint,
        }

        private static readonly CustomStyleProperty<Color> RingColor = new CustomStyleProperty<Color>("--ring-color");
        private static readonly CustomStyleProperty<Color> RingBright = new CustomStyleProperty<Color>("--ring-bright-color");

        private readonly VisualElement _layer;
        private readonly SpotlightDim _dim;
        private readonly VisualElement _ring;
        private readonly VisualElement _talk;
        private readonly AIFaceView _face;
        private readonly Label _text;
        private readonly Label _next;
        private readonly List<Rect> _holes = new List<Rect>();
        private readonly List<Rect> _clear = new List<Rect>();
        private readonly List<Rect> _keepVisible = new List<Rect>();
        private readonly List<Rect> _darkened = new List<Rect>();
        private Color _ringColor;
        private Color _ringBrightColor;
        private float _seconds;
        private bool _isExplaining;
        private readonly ShownValue<(string, int)> _shownText = new ShownValue<(string, int)>();

        public TutorialScreen(VisualElement root) : base(root)
        {
            _layer = Q("tutorial");
            _dim = Q<SpotlightDim>("dim");
            _ring = Q("ring");
            _talk = Q("talk");
            _face = Q<AIFaceView>("face");
            _text = Q<Label>("text");
            _next = Q<Label>("next");
            _next.text = TutorialWords.Next;
            root.WriteLabel("speaker", TutorialWords.Speaker);
            _dim.RegisterCallback<ClickEvent>(_ => OnClick());
            _ring.RegisterCallback<CustomStyleResolvedEvent>(OnRingStyle);
        }

        private void OnClick()
        {
            if (Context == null)
            {
                return;
            }

            if (_isExplaining)
            {
                Session.Tutorial.FinishExplanation();
            }
            else
            {
                Session.Speaker.Click();
            }
        }

        protected override void OnBind()
        {
            Hide();
        }

        public void Hide()
        {
            _isExplaining = false;
            _layer.Show(false);
        }

        public Rect CellsRect(CellRect cells)
        {
            Vector2 a = ScreenToPanel(App.GridOnScreen.PointOf(cells.Column, cells.EndRow));
            Vector2 b = ScreenToPanel(App.GridOnScreen.PointOf(cells.EndColumn, cells.Row));
            return Rect.MinMaxRect(Mathf.Min(a.x, b.x), Mathf.Min(a.y, b.y), Mathf.Max(a.x, b.x), Mathf.Max(a.y, b.y));
        }

        public void Refresh(in TutorialFrame frame)
        {
            TutorialFlow guide = Session.Tutorial;
            ITalkSpeaker speaker = Session.Speaker;
            _seconds += App.UIDelta;
            bool isTalking = speaker.ShowsTalk;
            if (!guide.IsActive && !isTalking)
            {
                Hide();
                return;
            }

            _layer.Show(true);
            TutorialCue cue = App.Cue;
            Rect? cueArea = cue.Target?.AreaOn(this);
            Rect? target = guide.IsActive && !isTalking && !guide.IsBlacking && cueArea.HasValue ? Local(cueArea.Value, RingPadding) : (Rect?)null;
            _isExplaining = target.HasValue && App.IsExplaining;
            Rect? shown = isTalking && frame.Showcase.IsLaidOut() ? Local(frame.Showcase.worldBound, RingPadding) : (Rect?)null;
            float focusK = guide.FocusWeight();
            Rect? facility = CollectCuts(guide, cue, target, frame.Menus, frame.Time);
            if (shown.HasValue)
            {
                _clear.Add(shown.Value);
                _keepVisible.Add(shown.Value);
            }

            ShadeOutside(isTalking || target.HasValue, facility, focusK, frame.Controls);
            ShowRing(target ?? shown);
            if (isTalking)
            {
                ShowTalking(speaker, focusK, shown);
                return;
            }

            ShowGuide(guide, cue, target, frame.Field);
        }

        private void ShowTalking(ITalkSpeaker speaker, float focusK, Rect? shown)
        {
            _talk.Show(true);
            ShowLine(speaker, focusK > 0f ? TalkPlace.Low : TalkPlace.Middle);
            if (shown.HasValue)
            {
                PlaceTalk(TalkPlace.At, PromptPosition(shown.Value));
            }
        }

        private void ShowGuide(TutorialFlow guide, TutorialCue cue, Rect? target, Rect field)
        {
            if (target.HasValue)
            {
                string text = _isExplaining ? cue.Explanation : guide.Prompt(cue.Step, cue.Material);
                ShowPrompt(text, PromptPosition(target.Value), false);
            }
            else if (!guide.IsBlacking && guide.Hint != null)
            {
                ShowPrompt(guide.Hint, HintPosition(Local(field, 0f)), true);
            }
            else
            {
                _talk.Show(false);
            }
        }

        private Rect? CollectCuts(TutorialFlow guide, TutorialCue cue, Rect? target, IReadOnlyList<Rect> menus, IReadOnlyList<Rect> time)
        {
            _holes.Clear();
            _clear.Clear();
            _keepVisible.Clear();
            foreach (Rect menu in menus)
            {
                _holes.Add(Local(menu, 0f));
            }

            if (guide.AllowsTime && !guide.ShowsTalk && !_isExplaining)
            {
                foreach (Rect button in time)
                {
                    _holes.Add(Local(button, 0f));
                }
            }

            if (target.HasValue)
            {
                (_isExplaining ? _clear : _holes).Add(target.Value);
                _keepVisible.Add(target.Value);
                KeepInView(cue.Context);
                KeepInView(cue.Reveal);
            }

            Facility focus = guide.FocusFacility;
            if (focus == null)
            {
                return null;
            }

            Rect facility = Local(CellsRect(focus.Rect), 0f);
            _clear.Add(facility);
            return facility;
        }

        private void KeepInView(VisualElement element)
        {
            if (!element.IsLaidOut())
            {
                return;
            }

            Rect area = Local(element.worldBound, 0f);
            _clear.Add(area);
            _keepVisible.Add(area);
        }

        private void ShadeOutside(bool isShadingAll, Rect? facility, float focusK, IReadOnlyList<Rect> controls)
        {
            _dim.SetCuts(_holes, _clear);
            if (isShadingAll)
            {
                _dim.ShadeAll(facility, focusK);
                return;
            }

            _darkened.Clear();
            foreach (Rect control in controls)
            {
                Rect local = Local(control, 0f);
                if (!_holes.Exists(hole => hole.Overlaps(local)))
                {
                    _darkened.Add(local);
                }
            }

            _dim.ShadeOnly(_darkened, facility, focusK);
        }

        private void ShowRing(Rect? target)
        {
            _ring.Display(target.HasValue);
            if (!target.HasValue)
            {
                return;
            }

            Place(_ring, target.Value);
            Color ring = Color.Lerp(_ringColor, _ringBrightColor, 0.5f - 0.5f * Mathf.Cos(_seconds * Mathf.PI * 2f / PulseSeconds));
            _ring.style.borderTopColor = ring;
            _ring.style.borderRightColor = ring;
            _ring.style.borderBottomColor = ring;
            _ring.style.borderLeftColor = ring;
        }

        private void ShowPrompt(string prompt, Vector2 at, bool isHint)
        {
            bool hasPrompt = !string.IsNullOrEmpty(prompt);
            _talk.Show(hasPrompt);
            if (!hasPrompt)
            {
                return;
            }

            SetText(prompt, prompt.Length);
            _next.Show(_isExplaining);
            _face.Draw(_seconds, AIFace.Normal, false);
            PlaceTalk(isHint ? TalkPlace.Hint : TalkPlace.At, at);
        }

        private void ShowLine(ITalkSpeaker speaker, TalkPlace place)
        {
            SpokenLine line = speaker.Line;
            SetText(line.Wording(Data), speaker.ShownChars);
            _next.Show(speaker.IsLineShown);
            _face.Draw(_seconds, line.Face, !speaker.IsLineShown);
            PlaceTalk(place, default);
        }

        private void PlaceTalk(TalkPlace place, Vector2 at)
        {
            _talk.EnableInClassList("tutorial__talk--middle", place == TalkPlace.Middle);
            _talk.EnableInClassList("tutorial__talk--low", place == TalkPlace.Low);
            _talk.EnableInClassList("tutorial__talk--hint", place == TalkPlace.Hint);
            bool isPlaced = place == TalkPlace.At || place == TalkPlace.Hint;
            _talk.style.left = isPlaced ? new StyleLength(at.x) : new StyleLength(StyleKeyword.Null);
            _talk.style.top = isPlaced ? new StyleLength(at.y) : new StyleLength(StyleKeyword.Null);
        }

        private void SetText(string source, int shown)
        {
            if (!_shownText.Change((source, shown)))
            {
                return;
            }

            _text.text = source.Marked(Data, shown);
        }

        private Vector2 PromptPosition(Rect target)
        {
            var all = new Rect(0f, 0f, _layer.layout.width, _layer.layout.height);
            float width = _talk.resolvedStyle.width;
            float height = _talk.resolvedStyle.height;
            bool above = target.center.y > all.center.y;
            float near = above ? target.yMin - Margin - height : target.yMax + Margin;
            Span<float> ys = stackalloc float[] { near, target.center.y - height * 0.5f, all.yMin, all.yMax };
            Span<float> xs = stackalloc float[] { target.xMin, target.xMin - Margin - width, target.xMax + Margin };
            Vector2 best = default;
            float least = float.MaxValue;
            foreach (float yCandidate in ys)
            {
                float y = Mathf.Clamp(yCandidate, all.yMin + Margin * 0.5f, all.yMax - Margin * 0.5f - height);
                foreach (float xCandidate in xs)
                {
                    float x = Mathf.Clamp(xCandidate, all.xMin + Margin, all.xMax - Margin - width);
                    var at = new Rect(x, y, width, height);
                    float overlap = _keepVisible.Sum(keep => Overlap(at, keep));
                    if (overlap < least - 0.5f)
                    {
                        least = overlap;
                        best = at.position;
                    }
                }
            }

            return best;
        }

        private Vector2 HintPosition(Rect field)
        {
            return new Vector2(field.xMax - Margin - HintWidth, field.yMin + Margin);
        }

        private static float Overlap(Rect a, Rect b)
        {
            float w = Mathf.Min(a.xMax, b.xMax) - Mathf.Max(a.xMin, b.xMin);
            float h = Mathf.Min(a.yMax, b.yMax) - Mathf.Max(a.yMin, b.yMin);
            return w > 0f && h > 0f ? w * h : 0f;
        }

        private static void Place(VisualElement element, Rect local)
        {
            element.style.left = local.xMin;
            element.style.top = local.yMin;
            element.style.width = local.width;
            element.style.height = local.height;
        }

        private Rect Local(Rect panel, float padding)
        {
            Rect r = _layer.WorldToLocal(panel);
            return Rect.MinMaxRect(r.xMin - padding, r.yMin - padding, r.xMax + padding, r.yMax + padding);
        }

        private Vector2 ScreenToPanel(Vector2 screen)
        {
            return Root.panel.ScreenToPanel(screen);
        }

        private void OnRingStyle(CustomStyleResolvedEvent eventData)
        {
            eventData.customStyle.TryGetValue(RingColor, out _ringColor);
            eventData.customStyle.TryGetValue(RingBright, out _ringBrightColor);
        }
    }
}
