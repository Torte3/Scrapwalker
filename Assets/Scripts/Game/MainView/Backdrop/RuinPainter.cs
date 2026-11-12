using System.Collections.Generic;
using Scrapwalker.Core.Travel;
using Scrapwalker.Game.Definitions;
using Scrapwalker.Game.Look.Art;
using UnityEngine;

namespace Scrapwalker.Game.MainView.Backdrop
{
    public sealed class RuinPainter : WorldPainter
    {
        private readonly ArtParts _parts;
        private readonly GameData _data;
        private readonly Dictionary<RuinDefinition, float> _rightEdges = new Dictionary<RuinDefinition, float>();

        public RuinPainter(WorldScene scene, ArtParts parts, GameData data)
            : base(scene)
        {
            _parts = parts;
            _data = data;
        }

        public void DrawBack(RuinDefinition ruin, float x, Color tint)
        {
            Put(_data.Asset(ruin).Back, x, DrawBand.Ruins, tint);
        }

        public void DrawFront(RuinDefinition ruin, float x, DrawBand band, Color tint)
        {
            Put(_data.Asset(ruin).Front, x, band, tint);
        }

        public float RightEdge(RuinDefinition ruin)
        {
            if (!_rightEdges.TryGetValue(ruin, out float right))
            {
                right = MeasureRightEdge(ruin);
                _rightEdges.Add(ruin, right);
            }

            return right;
        }

        private float MeasureRightEdge(RuinDefinition ruin)
        {
            ArtEntry entry = _data.Asset(ruin).Picture;
            if (entry == null)
            {
                return 0f;
            }

            float right = 0f;
            foreach (ArtLayer layer in entry.Layers)
            {
                Sprite sprite = layer.First?.Sprite;
                if (sprite != null)
                {
                    right = Mathf.Max(right, sprite.bounds.max.x);
                }
            }

            return right;
        }

        private void Put(ArtLayer layer, float x, DrawBand band, Color tint)
        {
            if (layer == null)
            {
                return;
            }

            Canvas.Save();
            Canvas.Translate(x, 0f);
            _parts.Put(layer, band, tint);
            Canvas.Restore();
        }
    }
}
