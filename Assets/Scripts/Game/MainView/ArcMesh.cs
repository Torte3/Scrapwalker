using System.Collections.Generic;
using UnityEngine;

namespace Scrapwalker.Game.MainView
{
    public sealed class ArcMesh : MonoBehaviour
    {
        private const DrawBand Band = DrawBand.Effects;

        private readonly List<Vector3> _vertices = new List<Vector3>(8192);
        private readonly List<Color> _colors = new List<Color>(8192);
        private readonly List<int> _triangles = new List<int>(16384);
        private readonly List<Vector2> _clipPoly = new List<Vector2>(16);
        private readonly List<Vector2> _clipTemporary = new List<Vector2>(16);
        private CanvasStack _canvas;
        private Material _material;
        private Mesh _mesh;
        private MeshRenderer _renderer;
        private int _order;
        private bool _wasEmpty;
        private bool _isLinear;
        private bool _hasClip;
        private Rect _clip;

        public void Initialize(CanvasStack canvas, Shader shader)
        {
            _canvas = canvas;
            _isLinear = QualitySettings.activeColorSpace == ColorSpace.Linear;
            _material = new Material(shader);
        }

        public void Begin(int order)
        {
            _order = order;
            if (_renderer != null)
            {
                _renderer.sortingOrder = order;
            }

            _vertices.Clear();
            _colors.Clear();
            _triangles.Clear();
            _hasClip = false;
        }

        public void End()
        {
            if (_mesh == null)
            {
                return;
            }

            bool isEmpty = _vertices.Count == 0;
            if (isEmpty && _wasEmpty)
            {
                return;
            }

            _wasEmpty = isEmpty;
            _mesh.Clear();
            _mesh.SetVertices(_vertices);
            _mesh.SetColors(_colors);
            _mesh.SetTriangles(_triangles, 0, false);
            _mesh.RecalculateBounds();
        }

        public void ClipRect(float x, float y, float w, float h)
        {
            Vector2 p0 = _canvas.ToCanvas(x, y);
            Vector2 p1 = _canvas.ToCanvas(x + w, y);
            Vector2 p2 = _canvas.ToCanvas(x + w, y + h);
            Vector2 p3 = _canvas.ToCanvas(x, y + h);
            float x0 = Mathf.Min(Mathf.Min(p0.x, p1.x), Mathf.Min(p2.x, p3.x));
            float x1 = Mathf.Max(Mathf.Max(p0.x, p1.x), Mathf.Max(p2.x, p3.x));
            float y0 = Mathf.Min(Mathf.Min(p0.y, p1.y), Mathf.Min(p2.y, p3.y));
            float y1 = Mathf.Max(Mathf.Max(p0.y, p1.y), Mathf.Max(p2.y, p3.y));
            _hasClip = true;
            _clip = Rect.MinMaxRect(x0, y0, x1, y1);
        }

        public void Unclip()
        {
            _hasClip = false;
        }

        public void StrokeArc(float cx, float cy, float r, float from, float to, float width, Color color, int segments)
        {
            Vector2 previous = new Vector2(cx + Mathf.Cos(from) * r, cy + Mathf.Sin(from) * r);
            for (int i = 1; i <= segments; i++)
            {
                float angle = from + (to - from) * i / segments;
                Vector2 next = new Vector2(cx + Mathf.Cos(angle) * r, cy + Mathf.Sin(angle) * r);
                Line(previous, next, width, color);
                previous = next;
            }
        }

        private void Line(Vector2 from, Vector2 to, float width, Color color)
        {
            Color tint = Tint(color);
            Vector2 a = _canvas.ToCanvas(from.x, from.y);
            Vector2 b = _canvas.ToCanvas(to.x, to.y);
            float w = width * _canvas.Transform.ScaleX;
            Vector2 d = b - a;
            float length = d.magnitude;
            if (length < 0.0001f)
            {
                return;
            }

            Vector2 n = new Vector2(-d.y, d.x) / length * (w * 0.5f);
            EmitMappedTriangle(a + n, b + n, b - n, tint);
            EmitMappedTriangle(a + n, b - n, a - n, tint);
        }

        private Color Tint(Color color)
        {
            color.a *= _canvas.Alpha;
            if (_isLinear)
            {
                color.r = Mathf.GammaToLinearSpace(color.r);
                color.g = Mathf.GammaToLinearSpace(color.g);
                color.b = Mathf.GammaToLinearSpace(color.b);
            }

            return color;
        }

        private void EmitMappedTriangle(Vector2 a, Vector2 b, Vector2 c, Color color)
        {
            if (!_hasClip)
            {
                EmitTriangle(a, b, c, color);
                return;
            }

            _clipPoly.Clear();
            _clipPoly.Add(a);
            _clipPoly.Add(b);
            _clipPoly.Add(c);
            ClipEdge(_clipPoly, _clipTemporary, 0, _clip.xMin, true);
            ClipEdge(_clipPoly, _clipTemporary, 0, _clip.xMax, false);
            ClipEdge(_clipPoly, _clipTemporary, 1, _clip.yMin, true);
            ClipEdge(_clipPoly, _clipTemporary, 1, _clip.yMax, false);
            for (int i = 1; i + 1 < _clipPoly.Count; i++)
            {
                EmitTriangle(_clipPoly[0], _clipPoly[i], _clipPoly[i + 1], color);
            }
        }

        private void EmitTriangle(Vector2 a, Vector2 b, Vector2 c, Color color)
        {
            if (_mesh == null)
            {
                _mesh = NewMesh();
            }

            int first = _vertices.Count;
            _vertices.Add(new Vector3(a.x, a.y, 0f));
            _vertices.Add(new Vector3(b.x, b.y, 0f));
            _vertices.Add(new Vector3(c.x, c.y, 0f));
            _colors.Add(color);
            _colors.Add(color);
            _colors.Add(color);
            _triangles.Add(first);
            _triangles.Add(first + 1);
            _triangles.Add(first + 2);
        }

        private Mesh NewMesh()
        {
            var meshObject = new GameObject("Mesh " + Band, typeof(MeshFilter), typeof(MeshRenderer));
            meshObject.transform.SetParent(transform, false);
            var mesh = new Mesh { indexFormat = UnityEngine.Rendering.IndexFormat.UInt32 };
            mesh.MarkDynamic();
            meshObject.GetComponent<MeshFilter>().sharedMesh = mesh;
            MeshRenderer meshRenderer = meshObject.GetComponent<MeshRenderer>();
            meshRenderer.sharedMaterial = _material;
            meshRenderer.sortingLayerID = Band.LayerID();
            meshRenderer.sortingOrder = _order;
            _renderer = meshRenderer;
            meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            meshRenderer.receiveShadows = false;
            return mesh;
        }

        internal static void ClipEdge(List<Vector2> poly, List<Vector2> temporary, int axis, float bound, bool keepGreater)
        {
            temporary.Clear();
            int count = poly.Count;
            for (int i = 0; i < count; i++)
            {
                Vector2 current = poly[i];
                Vector2 previous = poly[(i + count - 1) % count];
                bool isCurrentIn = IsInside(current[axis], bound, keepGreater);
                bool isPreviousIn = IsInside(previous[axis], bound, keepGreater);
                if (isCurrentIn)
                {
                    if (!isPreviousIn)
                    {
                        temporary.Add(Intersect(previous, current, axis, bound));
                    }

                    temporary.Add(current);
                }
                else if (isPreviousIn)
                {
                    temporary.Add(Intersect(previous, current, axis, bound));
                }
            }

            poly.Clear();
            poly.AddRange(temporary);
        }

        private static bool IsInside(float value, float bound, bool keepGreater)
        {
            return keepGreater ? value >= bound : value <= bound;
        }

        private static Vector2 Intersect(Vector2 a, Vector2 b, int axis, float bound)
        {
            float valueA = a[axis];
            float valueB = b[axis];
            float t = Mathf.Approximately(valueB, valueA) ? 0f : (bound - valueA) / (valueB - valueA);
            return a + (b - a) * t;
        }

        private void OnDestroy()
        {
            if (_mesh != null)
            {
                Destroy(_mesh);
            }

            if (_material != null)
            {
                Destroy(_material);
            }
        }
    }
}
