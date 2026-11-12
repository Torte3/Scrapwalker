using UnityEngine;

namespace Scrapwalker.Game.MainView
{
    public readonly struct CanvasTransform
    {
        public static readonly CanvasTransform Identity = new CanvasTransform(1f, 0f, 0f, 1f, 0f, 0f);

        public CanvasTransform(float a, float b, float c, float d, float tx, float ty)
        {
            A = a;
            B = b;
            C = c;
            D = d;
            Tx = tx;
            Ty = ty;
        }

        public float A { get; }
        public float B { get; }
        public float C { get; }
        public float D { get; }
        public float Tx { get; }
        public float Ty { get; }

        public float Angle => Mathf.Atan2(B, A);
        public float ScaleX => Mathf.Sqrt(A * A + B * B);
        public float Determinant => A * D - B * C;

        public Vector2 Map(float x, float y)
        {
            return new Vector2(A * x + C * y + Tx, B * x + D * y + Ty);
        }

        public Vector2 Unmap(Vector2 p)
        {
            float determinant = Determinant;
            if (Mathf.Abs(determinant) < 1e-8f)
            {
                return Vector2.zero;
            }

            float x = p.x - Tx;
            float y = p.y - Ty;
            return new Vector2((D * x - C * y) / determinant, (-B * x + A * y) / determinant);
        }

        public CanvasTransform Translated(float x, float y)
        {
            return new CanvasTransform(A, B, C, D, Tx + A * x + C * y, Ty + B * x + D * y);
        }

        public CanvasTransform Rotated(float angle)
        {
            float cos = Mathf.Cos(angle);
            float sin = Mathf.Sin(angle);
            return new CanvasTransform(A * cos + C * sin, B * cos + D * sin, -A * sin + C * cos, -B * sin + D * cos, Tx, Ty);
        }

        public CanvasTransform Scaled(float sx, float sy)
        {
            return new CanvasTransform(A * sx, B * sx, C * sy, D * sy, Tx, Ty);
        }
    }
}
