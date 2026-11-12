using Scrapwalker.Core.Items;
using Scrapwalker.Game.Definitions;
using Scrapwalker.Game.Look.Art;
using UnityEngine;

namespace Scrapwalker.Game.MainView.Effects
{
    public sealed class ItemIcons : WorldPainter
    {
        private const float ShadowRadiusX = 16f;
        private const float ShadowRadiusY = 2.5f;

        private readonly ArtParts _parts;
        private readonly GameData _data;

        public ItemIcons(WorldScene scene, ArtParts parts, GameData data)
            : base(scene)
        {
            _parts = parts;
            _data = data;
        }

        public void DrawOnFloor(MaterialDefinition type, float x, float y, float scale, DrawBand band)
        {
            _parts.Shadow(x, y, ShadowRadiusX * scale, ShadowRadiusY * scale, Color.white, band);
            Draw(type, x, y, scale, band);
        }

        public void Draw(MaterialDefinition type, float x, float y, float scale, DrawBand band)
        {
            ArtLayer icon = _data.Asset(type).IconArt;
            Canvas.Save();
            Canvas.Translate(x, y);
            Canvas.Scale(scale, scale);
            _parts.Put(icon, band);
            Canvas.Restore();
        }
    }
}
