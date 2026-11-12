using UnityEngine;
using UnityEngine.UIElements;

namespace Scrapwalker.Game.UI.Widgets
{
    public sealed class PanZoomView
    {
        public const float MinZoom = 0.4f;
        public const float MaxZoom = 1.6f;
        private const float ZoomStep = 1.1f;
        private const float FitMargin = 16f;
        private const float DragThreshold = 4f;

        private readonly VisualElement _port;
        private readonly VisualElement _content;
        private Vector2 _contentSize;
        private Vector2 _pan;
        private float _zoom = 1f;
        private bool _fitPending;
        private int _pressID = -1;
        private bool _dragging;
        private Vector2 _pressAt;
        private Vector2 _last;

        public PanZoomView(VisualElement port, VisualElement content)
        {
            _port = port;
            _content = content;
            _port.RegisterCallback<PointerDownEvent>(OnPointerDown, TrickleDown.TrickleDown);
            _port.RegisterCallback<PointerMoveEvent>(OnPointerMove, TrickleDown.TrickleDown);
            _port.RegisterCallback<PointerUpEvent>(OnPointerUp, TrickleDown.TrickleDown);
            _port.RegisterCallback<PointerCaptureOutEvent>(_ => EndDrag());
            _port.RegisterCallback<WheelEvent>(OnWheel);
            _port.RegisterCallback<GeometryChangedEvent>(_ => FitIfPending());
        }

        public void SetContentSize(Vector2 size)
        {
            _contentSize = size;
            _content.style.width = size.x;
            _content.style.height = size.y;
        }

        public void Fit()
        {
            _fitPending = true;
            FitIfPending();
        }

        public void ZoomAt(Vector2 at, float wheel)
        {
            if (Mathf.Approximately(wheel, 0f))
            {
                return;
            }

            float zoom = Mathf.Clamp(_zoom * (wheel > 0f ? ZoomStep : 1f / ZoomStep), MinZoom, MaxZoom);
            _pan = at - (at - _pan) * (zoom / _zoom);
            _zoom = zoom;
            _fitPending = false;
            ApplyViewTransform();
        }

        public void EndDrag()
        {
            _pressID = -1;
            _dragging = false;
        }

        private void FitIfPending()
        {
            Rect view = _port.contentRect;
            if (!_fitPending || float.IsNaN(view.width) || view.width <= 0f || view.height <= 0f)
            {
                return;
            }

            _fitPending = false;
            float zx = (view.width - FitMargin * 2f) / Mathf.Max(1f, _contentSize.x);
            float zy = (view.height - FitMargin * 2f) / Mathf.Max(1f, _contentSize.y);
            _zoom = Mathf.Clamp(Mathf.Min(zx, zy, 1f), MinZoom, MaxZoom);
            _pan = new Vector2((view.width - _contentSize.x * _zoom) * 0.5f, (view.height - _contentSize.y * _zoom) * 0.5f);
            ApplyViewTransform();
        }

        private void OnWheel(WheelEvent eventData)
        {
            ZoomAt(_port.WorldToLocal(eventData.mousePosition), -eventData.delta.y);
            eventData.StopPropagation();
        }

        private void OnPointerDown(PointerDownEvent eventData)
        {
            if (eventData.button != 0)
            {
                return;
            }

            _pressID = eventData.pointerId;
            _dragging = false;
            _pressAt = _port.WorldToLocal(eventData.position);
            _last = _pressAt;
        }

        private void OnPointerMove(PointerMoveEvent eventData)
        {
            if (eventData.pointerId != _pressID)
            {
                return;
            }

            Vector2 at = _port.WorldToLocal(eventData.position);
            if (!_dragging)
            {
                if ((at - _pressAt).sqrMagnitude < DragThreshold * DragThreshold)
                {
                    return;
                }

                _dragging = true;
                _port.CapturePointer(eventData.pointerId);
            }

            _pan += at - _last;
            _last = at;
            _fitPending = false;
            ApplyViewTransform();
        }

        private void OnPointerUp(PointerUpEvent eventData)
        {
            if (eventData.pointerId != _pressID)
            {
                return;
            }

            if (_dragging && _port.HasPointerCapture(eventData.pointerId))
            {
                _port.ReleasePointer(eventData.pointerId);
            }

            EndDrag();
        }

        private void ApplyViewTransform()
        {
            _content.style.translate = new Translate(_pan.x, _pan.y);
            _content.style.scale = new Scale(new Vector2(_zoom, _zoom));
        }
    }
}
