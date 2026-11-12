using System.Collections.Generic;
using Scrapwalker.Game.Look.Art;
using TMPro;
using UnityEngine;

namespace Scrapwalker.Game.MainView
{
    public sealed class WorldUI : MonoBehaviour
    {
        private const int CornerTexels = 8;
        private const float FontSizePerPixel = 10f;
        private const int FacilityTextOrder = WorldScene.LabelOrder - 1;

        private readonly Dictionary<float, Sprite> _rounds = new Dictionary<float, Sprite>();
        private readonly List<Object> _made = new List<Object>();
        private readonly List<TextMeshPro> _texts = new List<TextMeshPro>();
        private readonly List<MeshRenderer> _textRenderers = new List<MeshRenderer>();
        private Sprite _white;
        private CanvasStack _canvas;
        private Transform _shapeRoot;
        private Transform _textRoot;
        private TextMeshPro _measure;
        private Material _material;
        private TMP_FontAsset _font;
        private int _textUsed;
        private DrawOrder _order;
        private RendererPool _rects;
        private DrawBand? _band;
        private int _defaultOrder;

        public void Initialize(CanvasStack canvas, DrawOrder order, Sprite white, Shader shader, TMP_FontAsset font)
        {
            _canvas = canvas;
            _order = order;
            _white = white;
            _material = new Material(shader);
            _font = font;
            _shapeRoot = NewRoot("Shapes", 1f);
            _textRoot = NewRoot("Text", -1f);
            _rects = new RendererPool(_shapeRoot, "Shape");
            LayerDefault(0);
        }

        private Transform NewRoot(string name, float flipY)
        {
            var go = new GameObject(name);
            go.transform.SetParent(transform, false);
            go.transform.localScale = new Vector3(1f, flipY, 1f);
            return go.transform;
        }

        public void Layer(DrawBand band)
        {
            _band = band;
        }

        public void LayerDefault(int order)
        {
            _band = null;
            _defaultOrder = order;
        }

        public void Begin()
        {
            _rects.Begin();
            _textUsed = 0;
        }

        public void End()
        {
            _rects.End();

            for (int i = _textUsed; i < _texts.Count; i++)
            {
                if (_texts[i].gameObject.activeSelf)
                {
                    _texts[i].gameObject.SetActive(false);
                }
            }
        }

        public void FillRect(float x, float y, float w, float h, Color color)
        {
            if (w <= 0f || h <= 0f)
            {
                return;
            }

            SpriteRenderer shape = NextRect(_white, SpriteDrawMode.Simple, color);
            Place(shape, x, y);
            Vector2 size = _white.bounds.size;
            shape.transform.localScale = new Vector3(w / size.x, -h / size.y, 1f);
        }

        public void FillRoundRect(float x, float y, float w, float h, float radius, Color color)
        {
            if (w <= 0f || h <= 0f)
            {
                return;
            }

            SpriteRenderer shape = NextRect(Round(radius), SpriteDrawMode.Sliced, color);
            Place(shape, x, y);
            shape.transform.localScale = new Vector3(1f, -1f, 1f);
            shape.size = new Vector2(w, h);
        }

        public void HorizontalLine(float x1, float x2, float y, float width, Color color)
        {
            FillRect(Mathf.Min(x1, x2), y - width * 0.5f, Mathf.Abs(x2 - x1), width, color);
        }

        public void VerticalLine(float x, float y1, float y2, float width, Color color)
        {
            FillRect(x - width * 0.5f, Mathf.Min(y1, y2), width, Mathf.Abs(y2 - y1), color);
        }

        private void AxisLine(Vector2 from, Vector2 to, float width, Color color)
        {
            if (Mathf.Approximately(from.x, to.x))
            {
                VerticalLine(from.x, from.y, to.y, width, color);
            }
            else
            {
                HorizontalLine(from.x, to.x, from.y, width, color);
            }
        }

        public void StrokeRect(float x, float y, float w, float h, float width, Color color)
        {
            HorizontalLine(x - width * 0.5f, x + w + width * 0.5f, y, width, color);
            HorizontalLine(x - width * 0.5f, x + w + width * 0.5f, y + h, width, color);
            VerticalLine(x, y + width * 0.5f, y + h - width * 0.5f, width, color);
            VerticalLine(x + w, y + width * 0.5f, y + h - width * 0.5f, width, color);
        }

        public void DashedRect(float x, float y, float w, float h, float width, Color color, float dash, float gap, float offset = 0f)
        {
            DashedLine(new Vector2(x, y), new Vector2(x + w, y), width, color, dash, gap, offset);
            DashedLine(new Vector2(x + w, y), new Vector2(x + w, y + h), width, color, dash, gap, offset);
            DashedLine(new Vector2(x + w, y + h), new Vector2(x, y + h), width, color, dash, gap, offset);
            DashedLine(new Vector2(x, y + h), new Vector2(x, y), width, color, dash, gap, offset);
        }

        private void DashedLine(Vector2 from, Vector2 to, float width, Color color, float dash, float gap, float offset)
        {
            float length = Vector2.Distance(from, to);
            if (length < 0.001f)
            {
                return;
            }

            Vector2 direction = (to - from) / length;
            float pitch = dash + gap;
            float t = -Mathf.Repeat(offset, pitch);
            while (t < length)
            {
                float a = Mathf.Max(0f, t);
                float b = Mathf.Min(length, t + dash);
                if (b > a)
                {
                    AxisLine(from + direction * a, from + direction * b, width, color);
                }

                t += pitch;
            }
        }

        public void Text(float x, float y, string text, float pixels, Color color, FontStyles style = FontStyles.Normal, TextAlignmentOptions align = TextAlignmentOptions.BottomLeft, int sortingOrder = FacilityTextOrder)
        {
            if (_textUsed == _texts.Count)
            {
                TextMeshPro created = NewText("Label");
                created.overflowMode = TextOverflowModes.Overflow;
                created.rectTransform.sizeDelta = new Vector2(400f, 40f);
                _texts.Add(created);
                _textRenderers.Add(created.GetComponent<MeshRenderer>());
            }

            TextMeshPro label = _texts[_textUsed];
            MeshRenderer renderer = _textRenderers[_textUsed];
            _textUsed++;
            if (!label.gameObject.activeSelf)
            {
                label.gameObject.SetActive(true);
            }

            Vector2 p = _canvas.ToCanvas(x, y);
            label.rectTransform.localPosition = new Vector3(p.x, -p.y, 0f);
            label.rectTransform.localRotation = Quaternion.Euler(0f, 0f, -_canvas.Transform.Angle * Mathf.Rad2Deg);
            label.rectTransform.pivot = PivotFor(align);
            label.alignment = align;
            label.fontSize = pixels * FontSizePerPixel;
            label.fontStyle = style;
            color.a *= _canvas.Alpha;
            label.color = color;
            if (label.text != text)
            {
                label.text = text;
            }

            renderer.sortingOrder = sortingOrder;
        }

        public float TextWidth(string text, float pixels, FontStyles style = FontStyles.Normal)
        {
            if (_measure == null)
            {
                _measure = NewText("Measure");
                _measure.gameObject.SetActive(false);
                _measure.text = string.Empty;
            }

            _measure.fontSize = pixels * FontSizePerPixel;
            _measure.fontStyle = style;
            return _measure.GetPreferredValues(text).x;
        }

        private TextMeshPro NewText(string name)
        {
            var go = new GameObject(name, typeof(TextMeshPro));
            go.transform.SetParent(_textRoot, false);
            TextMeshPro label = go.GetComponent<TextMeshPro>();
            label.font = _font;
            label.textWrappingMode = TextWrappingModes.NoWrap;
            return label;
        }

        private static Vector2 PivotFor(TextAlignmentOptions align)
        {
            switch (align)
            {
                case TextAlignmentOptions.Center:
                case TextAlignmentOptions.Midline:
                    return new Vector2(0.5f, 0.5f);
                case TextAlignmentOptions.Bottom:
                    return new Vector2(0.5f, 0f);
                case TextAlignmentOptions.BottomRight:
                    return new Vector2(1f, 0f);
                case TextAlignmentOptions.Top:
                    return new Vector2(0.5f, 1f);
                case TextAlignmentOptions.TopLeft:
                    return new Vector2(0f, 1f);
                default:
                    return new Vector2(0f, 0f);
            }
        }

        private void Place(SpriteRenderer shape, float x, float y)
        {
            Vector2 p = _canvas.ToCanvas(x, y);
            shape.transform.localPosition = new Vector3(p.x, p.y, 0f);
            shape.transform.localRotation = Quaternion.Euler(0f, 0f, _canvas.Transform.Angle * Mathf.Rad2Deg);
        }

        private SpriteRenderer NextRect(Sprite sprite, SpriteDrawMode mode, Color color)
        {
            SpriteRenderer shape = _rects.Next();
            shape.sharedMaterial = _material;
            if (shape.sprite != sprite)
            {
                shape.sprite = sprite;
            }

            shape.drawMode = mode;
            color.a *= _canvas.Alpha;
            shape.color = color;
            shape.sortingLayerID = _band.HasValue ? _band.Value.LayerID() : 0;
            shape.sortingOrder = _band.HasValue ? _order.Next(_band.Value) : _defaultOrder;

            return shape;
        }

        private Sprite Round(float radius)
        {
            if (_rounds.TryGetValue(radius, out Sprite sprite))
            {
                return sprite;
            }

            int r = CornerTexels;
            int size = r * 2 + 1;
            var pixels = new Color32[size * size];
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    var texel = new Vector2(x + 0.5f, y + 0.5f);
                    var corner = new Vector2(Mathf.Clamp(texel.x, r, size - r), Mathf.Clamp(texel.y, r, size - r));
                    float a = Mathf.Clamp01(r + 0.5f - Vector2.Distance(texel, corner));
                    pixels[y * size + x] = new Color32(255, 255, 255, (byte)Mathf.RoundToInt(a * 255f));
                }
            }

            sprite = MakeSprite("WorldUiRound", size, pixels, r / Mathf.Max(0.01f, radius), new Vector4(r, r, r, r));
            _rounds[radius] = sprite;
            return sprite;
        }

        private Sprite MakeSprite(string name, int size, Color32[] pixels, float pixelsPerUnit, Vector4 border)
        {
            var texture = new Texture2D(size, size, TextureFormat.RGBA32, false) { filterMode = FilterMode.Bilinear, wrapMode = TextureWrapMode.Clamp, name = name };
            texture.SetPixels32(pixels);
            texture.Apply();
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0f, 1f), pixelsPerUnit, 0, SpriteMeshType.FullRect, border);
            _made.Add(texture);
            _made.Add(sprite);
            return sprite;
        }

        private void OnDestroy()
        {
            if (_material != null)
            {
                Destroy(_material);
            }

            foreach (Object made in _made)
            {
                Destroy(made);
            }
        }
    }
}
