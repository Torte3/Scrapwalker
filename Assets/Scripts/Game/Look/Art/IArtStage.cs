using Scrapwalker.Game.Definitions;
using UnityEngine;

namespace Scrapwalker.Game.Look.Art
{
    public interface IArtStage
    {
        void Save();

        void Restore();

        void Translate(float x, float y);

        void Rotate(float angle);

        void RotateAbout(Vector2 pivot, float angle);

        void Scale(float sx, float sy);

        Vector2 FromCanvas(Vector2 p);

        void Put(ArtLayer layer, ArtFrame frame, Color tint);

        void Disc(float x, float y, float radius, Color tint);

        void Glow(float x, float y, float radiusX, float radiusY, Color tint);

        void FillRect(float x, float y, float w, float h, Color tint);

        bool IsEvery(int frames);

        void Emit(in SparkBurst burst, Vector2 at);
    }
}
