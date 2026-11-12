using Scrapwalker.Core.Travel.Talk;
using UnityEngine;
using UnityEngine.UIElements;

namespace Scrapwalker.Game.UI.Screens.Tutorial
{
    [UxmlElement]
    public partial class AIFaceView : VisualElement
    {
        private const int Size = 100;
        private const float StepsPerSecond = 60f;
        private const float Unit = Size * 0.68f / 17f;
        private const int BlinkPeriodSteps = 150;
        private const int BlinkSteps = 6;
        private const int GlitchPeriodSteps = 90;
        private const int GlitchSteps = 5;
        private const int FlickerPeriodSteps = 233;
        private const int FlickerSteps = 3;
        private const int GlitchBandHeight = 12;
        private const float LeftEye = 5f;
        private const float RightEye = 11f;

        private static readonly CustomStyleProperty<Color> OuterProperty = new CustomStyleProperty<Color>("--face-outer");
        private static readonly CustomStyleProperty<Color> CaseProperty = new CustomStyleProperty<Color>("--face-case");
        private static readonly CustomStyleProperty<Color> ScreenProperty = new CustomStyleProperty<Color>("--face-screen");
        private static readonly CustomStyleProperty<Color> StandProperty = new CustomStyleProperty<Color>("--face-stand");
        private static readonly CustomStyleProperty<Color> EyeProperty = new CustomStyleProperty<Color>("--face-eye");
        private static readonly CustomStyleProperty<Color> GlitchProperty = new CustomStyleProperty<Color>("--face-glitch");

        private readonly Color32[] _pixels = new Color32[Size * Size];
        private readonly Color32[] _row = new Color32[Size];
        private readonly QuadMesh _mesh = new QuadMesh();
        private Color32 _outer;
        private Color32 _case;
        private Color32 _screen;
        private Color32 _stand;
        private Color32 _eye;
        private Color32 _glitch;

        public AIFaceView()
        {
            pickingMode = PickingMode.Ignore;
            generateVisualContent += Paint;
            RegisterCallback<CustomStyleResolvedEvent>(OnCustomStyle);
        }

        public void Draw(float seconds, AIFace face, bool isSpeaking)
        {
            int step = (int)(seconds * StepsPerSecond);
            DrawMonitor();
            if (face == AIFace.Trouble)
            {
                DrawTroubledFace();
            }
            else
            {
                DrawEyes(face, step % BlinkPeriodSteps < BlinkSteps && face != AIFace.Surprise);
                DrawMouth(face, isSpeaking && Mathf.Sin(step * 0.5f) > 0f);
            }

            if (step % GlitchPeriodSteps < GlitchSteps)
            {
                Glitch(step);
            }

            if (step % FlickerPeriodSteps < FlickerSteps)
            {
                Flicker();
            }

            MarkDirtyRepaint();
        }

        private void DrawMonitor()
        {
            Fill(0f, 0f, 1f, 1f, _outer);
            Fill(0.1f, 0.14f, 0.8f, 0.66f, _case);
            Fill(0.16f, 0.2f, 0.68f, 0.52f, _screen);
            Color32 line = Color32.Lerp(_screen, _eye, 0.08f);
            for (float y = 0.2f; y < 0.72f; y += 0.04f)
            {
                Fill(0.16f, y, 0.68f, 0.01f, line);
            }

            Fill(0.4f, 0.8f, 0.2f, 0.06f, _stand);
        }

        private void DrawTroubledFace()
        {
            Dot(LeftEye - 0.5f, 4.5f, 2f, 0.4f, _eye);
            Dot(LeftEye - 0.5f, 5.3f, 2f, 0.4f, _eye);
            Dot(RightEye - 0.5f, 4.5f, 2f, 0.4f, _eye);
            Dot(RightEye - 0.5f, 5.3f, 2f, 0.4f, _eye);

            Dot(6.5f, 9f, 1f, 0.5f, _eye);
            Dot(7.5f, 8.6f, 1f, 0.5f, _eye);
            Dot(8.5f, 9f, 1f, 0.5f, _eye);
            Dot(9.5f, 8.6f, 1f, 0.5f, _eye);
        }

        private void DrawEyes(AIFace face, bool isBlinking)
        {
            if (isBlinking)
            {
                Dot(LeftEye - 0.25f, 5f, 1.5f, 0.4f, _eye);
                Dot(RightEye - 0.25f, 5f, 1.5f, 0.4f, _eye);
            }
            else if (face == AIFace.Surprise)
            {
                Dot(LeftEye - 0.3f, 3.7f, 1.6f, 2.6f, _eye);
                Dot(RightEye - 0.3f, 3.7f, 1.6f, 2.6f, _eye);
            }
            else
            {
                Dot(LeftEye, 4f, 1f, 2f, _eye);
                Dot(RightEye, 4f, 1f, 2f, _eye);
            }
        }

        private void DrawMouth(AIFace face, bool isMouthOpen)
        {
            if (face == AIFace.Surprise)
            {
                Dot(8f, 8.3f, 1f, 1.4f, _eye);
            }
            else if (face == AIFace.Smile)
            {
                Dot(6.5f, 8f, 4f, 0.5f, _eye);
                Dot(7f, 8.5f, 3f, 0.5f, _eye);
                Dot(7.5f, 9f, 2f, 0.5f, _eye);
            }
            else if (isMouthOpen)
            {
                Dot(7f, 8.5f, 3f, 1.2f, _eye);
            }
            else
            {
                Dot(7f, 9f, 3f, 0.5f, _eye);
            }
        }

        private void Flicker()
        {
            for (int i = 0; i < _pixels.Length; i++)
            {
                _pixels[i] = Color32.Lerp(_pixels[i], _outer, 0.5f);
            }
        }

        private void OnCustomStyle(CustomStyleResolvedEvent eventData)
        {
            ICustomStyle style = eventData.customStyle;
            _outer = Resolve(style, OuterProperty);
            _case = Resolve(style, CaseProperty);
            _screen = Resolve(style, ScreenProperty);
            _stand = Resolve(style, StandProperty);
            _eye = Resolve(style, EyeProperty);
            _glitch = Resolve(style, GlitchProperty);
        }

        private static Color32 Resolve(ICustomStyle style, CustomStyleProperty<Color> property)
        {
            return style.TryGetValue(property, out Color color) ? (Color32)color : default;
        }

        private void Glitch(int step)
        {
            int top = step * 41 % (Size - 15);
            int shift = step % 2 == 1 ? 5 : -4;
            for (int y = top; y < top + GlitchBandHeight; y++)
            {
                int row = y * Size;
                for (int x = 0; x < Size; x++)
                {
                    _row[x] = _pixels[row + Mathf.Clamp(x - shift, 0, Size - 1)];
                }

                System.Array.Copy(_row, 0, _pixels, row, Size);
            }

            Fill(0f, (top + GlitchBandHeight) / (float)Size, 1f, 0.015f, Color32.Lerp(_pixels[(Size - 1) * Size], _glitch, 0.25f));
        }

        private void Dot(float cx, float cy, float cellWidth, float cellHeight, Color32 color)
        {
            float sx = Size * 0.16f;
            float sy = Size * 0.2f;
            Pixels(Mathf.RoundToInt(sx + cx * Unit), Mathf.RoundToInt(sy + cy * Unit), Mathf.CeilToInt(cellWidth * Unit), Mathf.CeilToInt(cellHeight * Unit), color);
        }

        private void Fill(float x, float y, float w, float h, Color32 color)
        {
            Pixels(Mathf.RoundToInt(x * Size), Mathf.RoundToInt(y * Size), Mathf.Max(1, Mathf.RoundToInt(w * Size)), Mathf.Max(1, Mathf.RoundToInt(h * Size)), color);
        }

        private void Pixels(int x0, int y0, int w, int h, Color32 color)
        {
            for (int y = Mathf.Max(0, y0); y < Mathf.Min(Size, y0 + h); y++)
            {
                int row = y * Size;
                for (int x = Mathf.Max(0, x0); x < Mathf.Min(Size, x0 + w); x++)
                {
                    _pixels[row + x] = color;
                }
            }
        }

        private void Paint(MeshGenerationContext context)
        {
            Rect area = contentRect;
            float cell = Mathf.Min(area.width, area.height) / Size;
            Vector2 origin = area.center - new Vector2(cell * Size * 0.5f, cell * Size * 0.5f);
            _mesh.Clear();
            for (int y = 0; y < Size; y++)
            {
                int row = y * Size;
                int start = 0;
                for (int x = 1; x <= Size; x++)
                {
                    if (x < Size && Same(_pixels[row + x], _pixels[row + start]))
                    {
                        continue;
                    }

                    _mesh.Add(new Rect(origin.x + start * cell, origin.y + y * cell, (x - start) * cell, cell), _pixels[row + start]);
                    start = x;
                }
            }

            _mesh.Write(context);
        }

        private static bool Same(Color32 a, Color32 b)
        {
            return a.r == b.r && a.g == b.g && a.b == b.b && a.a == b.a;
        }
    }
}
