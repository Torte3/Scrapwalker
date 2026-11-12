using UnityEngine;
using static Scrapwalker.Game.Look.Palette;
using static Scrapwalker.Game.Look.Pen;

namespace Scrapwalker.Game.MainView.Backdrop
{
    public sealed class SpacePainter : WorldPainter
    {
        private const int Stars = 260;
        private const float SolidY = -1800f;
        private const float StarFromY = -200f;
        private const float StarSpanY = 1600f;
        private const float DriftX = 0.004f;
        private const float DriftY = 0.04f;
        private const float SpaceLeft = -4000f;
        private const float SpaceWidth = 10000f;
        private const float SolidTop = -60000f;

        private readonly ArtParts _parts;
        private readonly Star[] _stars = new Star[Stars];

        public SpacePainter(WorldScene scene, ArtParts parts)
            : base(scene)
        {
            _parts = parts;
            for (int i = 0; i < Stars; i++)
            {
                _stars[i] = new Star(Hash(i * 1.37f), Hash(i * 2.71f), 0.6f + Hash(i * 5.3f) * 1.3f, 0.4f + Hash(i * 9.1f) * 0.6f);
            }
        }

        public void Draw(Rect view, float worldX, float lift)
        {
            _parts.StretchWidth(_parts.Art.Backdrop.Space, SpaceLeft, 0f, SpaceWidth, Color.white, DrawBand.Backdrop);
            _parts.FillRect(SpaceLeft, SolidTop, SpaceWidth, SolidY - SolidTop, SpaceBlack, DrawBand.Backdrop);
            if (view.yMin > StarFromY)
            {
                return;
            }

            float scale = view.height / WorldScene.ScreenHeight;
            for (int i = 0; i < Stars; i++)
            {
                Star star = _stars[i];
                float sx = Mathf.Repeat(star.X + worldX * DriftX / WorldScene.ScreenWidth, 1f);
                float sy = Mathf.Repeat(star.Y + lift * DriftY / WorldScene.ScreenHeight, 1f);
                float y = view.yMin + sy * view.height;
                float k = Mathf.Clamp01((StarFromY - y) / StarSpanY) * star.Alpha;
                if (k <= 0f)
                {
                    continue;
                }

                _parts.Disc(view.xMin + sx * view.width, y, star.Radius * scale, StarLight.WithAlpha(k), DrawBand.Backdrop);
            }
        }
    }
}
