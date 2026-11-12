using System;
using System.Collections.Generic;
using System.Linq;
using Scrapwalker.Core.Items;
using UnityEngine;
using UnityEngine.UIElements;

namespace Scrapwalker.Game.UI.Widgets.QualityFilters
{
    public sealed class QualityRangeSlider
    {
        private readonly VisualElement _strip;
        private readonly VisualElement[] _cells;
        private readonly QualityPips _pips;
        private ShownQualities _shown;
        private int _min;
        private int _max;
        private int _startMin;
        private int _startMax;
        private bool _isMovingLowEdge;
        private int _pointer = -1;
        private Action<Quality, Quality> _onChange;

        public QualityRangeSlider(VisualElement strip)
        {
            _strip = strip;
            List<VisualElement> cells = strip.Query(className: "quality-range__cell").ToList();
            if (cells.Count != QualityRules.Count)
            {
                throw new InvalidOperationException(strip.name + " の四角の数が品質の数と違う（UXML を見る）");
            }

            _cells = cells.ToArray();
            _pips = new QualityPips(_cells, "quality-range__edge--low", "quality-range__edge--high", "quality-range__cell--on");

            _strip.RegisterCallback<PointerDownEvent>(OnPointerDown);
            _strip.RegisterCallback<PointerMoveEvent>(OnPointerMove);
            _strip.RegisterCallback<PointerUpEvent>(OnPointerUp);
            _strip.RegisterCallback<PointerCaptureOutEvent>(_ => Cancel());
            _strip.RegisterCallback<DetachFromPanelEvent>(_ => Cancel());
        }

        private bool IsDragging => _pointer >= 0;

        public void Set(Quality min, Quality max, ShownQualities shown, Action<Quality, Quality> onChange)
        {
            _onChange = onChange;
            if (IsDragging)
            {
                return;
            }

            _shown = shown;
            _min = (int)min;
            _max = (int)max;
            Paint();
        }

        private void OnPointerDown(PointerDownEvent eventData)
        {
            if (eventData.button != 0 || IsDragging)
            {
                return;
            }

            int q = CellAt(eventData.position);
            _startMin = _min;
            _startMax = _max;
            _isMovingLowEdge = IsNearerLowEnd(q);
            _pointer = eventData.pointerId;
            _strip.CapturePointer(eventData.pointerId);
            Move(q);
            eventData.StopPropagation();
        }

        private void OnPointerMove(PointerMoveEvent eventData)
        {
            if (eventData.pointerId == _pointer)
            {
                Move(CellAt(eventData.position));
            }
        }

        private void OnPointerUp(PointerUpEvent eventData)
        {
            if (eventData.pointerId != _pointer)
            {
                return;
            }

            _pointer = -1;
            _strip.ReleasePointer(eventData.pointerId);
            if ((_min != _startMin || _max != _startMax) && _onChange != null)
            {
                _onChange((Quality)_min, (Quality)_max);
            }

            eventData.StopPropagation();
        }

        private void Cancel()
        {
            _pointer = -1;
        }

        private void Move(int q)
        {
            if (!IsShown(q))
            {
                return;
            }

            if (_isMovingLowEdge)
            {
                _min = Mathf.Min(q, _max);
            }
            else
            {
                _max = Mathf.Max(q, _min);
            }

            Paint();
        }

        private bool IsNearerLowEnd(int q)
        {
            if (!_shown.TryGetEnds((Quality)_min, (Quality)_max, out Quality low, out Quality high))
            {
                return q <= _min;
            }

            return q < (int)low || (q <= (int)high && q - (int)low <= (int)high - q);
        }

        private int CellAt(Vector2 position)
        {
            Vector2 local = _strip.WorldToLocal(position);
            Rect rect = _strip.contentRect;
            float t = rect.width > 0f ? (local.x - rect.xMin) / rect.width : 0f;
            List<int> shownCells = Enumerable.Range(0, _cells.Length).Where(IsShown).ToList();
            int index = Mathf.Clamp(Mathf.FloorToInt(t * shownCells.Count), 0, Mathf.Max(0, shownCells.Count - 1));
            return shownCells.Count > 0 ? shownCells[index] : 0;
        }

        private bool IsShown(int q)
        {
            return _shown.Contains((Quality)q);
        }

        private void Paint()
        {
            _pips.Paint(_shown, (Quality)_min, (Quality)_max, true, true);
        }
    }
}
