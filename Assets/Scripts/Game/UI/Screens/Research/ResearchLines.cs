using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Scrapwalker.Game.UI.Screens.Research
{
    [UxmlElement]
    public partial class ResearchLines : VisualElement
    {
        private static readonly CustomStyleProperty<Color> LineColor = new CustomStyleProperty<Color>("--line-color");
        private static readonly CustomStyleProperty<Color> DoneColor = new CustomStyleProperty<Color>("--line-done-color");
        private static readonly CustomStyleProperty<Color> LitColor = new CustomStyleProperty<Color>("--line-lit-color");
        private static readonly CustomStyleProperty<float> LineWidth = new CustomStyleProperty<float>("--line-width");
        private static readonly CustomStyleProperty<float> LitWidth = new CustomStyleProperty<float>("--lit-width");

        private readonly List<IReadOnlyList<Vector2>> _curves = new List<IReadOnlyList<Vector2>>();
        private readonly List<ResearchLineState> _states = new List<ResearchLineState>();
        private Color _lineColor = Color.gray;
        private Color _doneColor = Color.green;
        private Color _litColor = Color.white;
        private float _lineWidth = 1f;
        private float _litWidth = 2f;

        public ResearchLines()
        {
            pickingMode = PickingMode.Ignore;
            generateVisualContent += Draw;
            RegisterCallback<CustomStyleResolvedEvent>(OnStyle);
        }

        public void ClearCurves()
        {
            _curves.Clear();
            _states.Clear();
            MarkDirtyRepaint();
        }

        public void Add(IReadOnlyList<Vector2> points)
        {
            _curves.Add(points);
            _states.Add(ResearchLineState.Normal);
            MarkDirtyRepaint();
        }

        public void SetState(int curve, ResearchLineState state)
        {
            if (_states[curve] == state)
            {
                return;
            }

            _states[curve] = state;
            MarkDirtyRepaint();
        }

        private static (Vector2, Vector2) ControlPoints(Vector2 a, Vector2 b)
        {
            float dx = (b.x - a.x) * 0.5f;
            return (new Vector2(a.x + dx, a.y), new Vector2(b.x - dx, b.y));
        }

        private void OnStyle(CustomStyleResolvedEvent eventData)
        {
            ICustomStyle style = eventData.customStyle;
            style.TryGetValue(LineColor, out _lineColor);
            style.TryGetValue(DoneColor, out _doneColor);
            style.TryGetValue(LitColor, out _litColor);
            style.TryGetValue(LineWidth, out _lineWidth);
            style.TryGetValue(LitWidth, out _litWidth);
            MarkDirtyRepaint();
        }

        private void Draw(MeshGenerationContext context)
        {
            Painter2D painter = context.painter2D;
            painter.lineJoin = LineJoin.Round;
            painter.lineCap = LineCap.Butt;
            for (int c = 0; c < _curves.Count; c++)
            {
                if (_states[c] != ResearchLineState.Lit)
                {
                    Stroke(painter, _curves[c], _states[c] == ResearchLineState.Done ? _doneColor : _lineColor, _lineWidth);
                }
            }

            for (int c = 0; c < _curves.Count; c++)
            {
                if (_states[c] == ResearchLineState.Lit)
                {
                    Stroke(painter, _curves[c], _litColor, _litWidth);
                }
            }
        }

        private static void Stroke(Painter2D painter, IReadOnlyList<Vector2> points, Color color, float width)
        {
            if (points.Count < 2)
            {
                return;
            }

            painter.strokeColor = color;
            painter.lineWidth = width;
            painter.BeginPath();
            painter.MoveTo(points[0]);
            for (int k = 0; k + 1 < points.Count; k++)
            {
                (Vector2 p1, Vector2 p2) = ControlPoints(points[k], points[k + 1]);
                painter.BezierCurveTo(p1, p2, points[k + 1]);
            }

            painter.Stroke();
        }
    }
}
