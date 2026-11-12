using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace Scrapwalker.Game.UI.Screens.Tutorial
{
    [UxmlElement]
    public partial class SpotlightDim : VisualElement
    {
        private static readonly CustomStyleProperty<Color> DimColor = new CustomStyleProperty<Color>("--dim-color");
        private static readonly CustomStyleProperty<Color> FocusColor = new CustomStyleProperty<Color>("--focus-dim-color");

        private readonly List<Rect> _holes = new List<Rect>();
        private readonly List<Rect> _clear = new List<Rect>();
        private readonly List<Rect> _drawn = new List<Rect>();
        private readonly List<float> _ys = new List<float>();
        private readonly List<Rect> _cut = new List<Rect>();
        private readonly List<Rect> _darkened = new List<Rect>();
        private readonly QuadMesh _mesh = new QuadMesh();
        private Color _dim;
        private Color _focus;
        private bool _isShadingAll;
        private Rect? _facility;
        private float _focusK;

        public SpotlightDim()
        {
            pickingMode = PickingMode.Position;
            generateVisualContent += Draw;
            RegisterCallback<CustomStyleResolvedEvent>(OnCustomStyle);
        }

        public void SetCuts(List<Rect> holes, List<Rect> clear)
        {
            if (holes.SequenceEqual(_holes) && clear.SequenceEqual(_clear))
            {
                return;
            }

            _holes.Clear();
            _holes.AddRange(holes);
            _clear.Clear();
            _clear.AddRange(clear);
            _drawn.Clear();
            _drawn.AddRange(holes);
            _drawn.AddRange(clear);
            MarkDirtyRepaint();
        }

        public void ShadeAll(Rect? facility, float focusK)
        {
            SetShade(true, facility, focusK, Array.Empty<Rect>());
        }

        public void ShadeOnly(IReadOnlyList<Rect> darkened, Rect? facility, float focusK)
        {
            SetShade(false, facility, focusK, darkened);
        }

        private void SetShade(bool isShadingAll, Rect? facility, float focusK, IReadOnlyList<Rect> darkened)
        {
            if (isShadingAll == _isShadingAll && facility == _facility && Mathf.Approximately(focusK, _focusK) && darkened.SequenceEqual(_darkened))
            {
                return;
            }

            _isShadingAll = isShadingAll;
            _facility = facility;
            _focusK = focusK;
            _darkened.Clear();
            _darkened.AddRange(darkened);
            MarkDirtyRepaint();
        }

        public override bool ContainsPoint(Vector2 localPoint)
        {
            return base.ContainsPoint(localPoint) && !_holes.Exists(hole => hole.Contains(localPoint));
        }

        private void OnCustomStyle(CustomStyleResolvedEvent eventData)
        {
            eventData.customStyle.TryGetValue(DimColor, out _dim);
            eventData.customStyle.TryGetValue(FocusColor, out _focus);
            MarkDirtyRepaint();
        }

        private void Draw(MeshGenerationContext context)
        {
            _mesh.Clear();
            Color outside = OutsideColor();
            if (outside.a > 0f)
            {
                Bands(new Rect(0f, 0f, layout.width, layout.height), outside);
            }

            if (_facility.HasValue)
            {
                _mesh.Add(_facility.Value, new Color(_dim.r, _dim.g, _dim.b, _dim.a * (1f - _focusK)));
            }

            foreach (Rect control in _darkened)
            {
                _mesh.Add(control, _dim);
            }

            _mesh.Write(context);
        }

        private Color OutsideColor()
        {
            if (_facility.HasValue)
            {
                return Color.Lerp(_dim, _focus, _focusK);
            }

            return _isShadingAll ? _dim : Color.clear;
        }

        private void Bands(Rect all, Color color)
        {
            _ys.Clear();
            _ys.Add(all.yMin);
            _ys.Add(all.yMax);
            foreach (Rect hole in _drawn)
            {
                _ys.Add(Mathf.Clamp(hole.yMin, all.yMin, all.yMax));
                _ys.Add(Mathf.Clamp(hole.yMax, all.yMin, all.yMax));
            }

            _ys.Sort();
            for (int i = 0; i + 1 < _ys.Count; i++)
            {
                float y0 = _ys[i];
                float y1 = _ys[i + 1];
                if (y1 - y0 <= 0f)
                {
                    continue;
                }

                _cut.Clear();
                foreach (Rect hole in _drawn)
                {
                    if (hole.yMin <= y0 && hole.yMax >= y1)
                    {
                        _cut.Add(hole);
                    }
                }

                _cut.Sort((a, b) => a.xMin.CompareTo(b.xMin));
                float x = all.xMin;
                foreach (Rect hole in _cut)
                {
                    _mesh.Add(Rect.MinMaxRect(x, y0, Mathf.Max(x, hole.xMin), y1), color);
                    x = Mathf.Max(x, hole.xMax);
                }

                _mesh.Add(Rect.MinMaxRect(x, y0, all.xMax, y1), color);
            }
        }
    }
}
