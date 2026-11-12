using System.Collections.Generic;
using Scrapwalker.Game.Look.Art;
using Scrapwalker.Core.Outpost.Hulls;
using UnityEngine;
using static Scrapwalker.Game.MainView.WorldScene;

namespace Scrapwalker.Game.MainView.Outpost
{
    public sealed class CrackPainter : WorldPainter
    {
        private readonly ArtParts _parts;

        public CrackPainter(WorldScene scene, ArtParts parts)
            : base(scene)
        {
            _parts = parts;
        }

        public void Draw(IReadOnlyList<Crack> cracks)
        {
            for (int i = 0; i < cracks.Count; i++)
            {
                Crack crack = cracks[i];
                ArtLayer[] shapes = _parts.Art.Outpost.Cracks;
                ArtLayer layer = shapes[(int)Mathf.Repeat(crack.ID, shapes.Length)];
                Canvas.Save();
                Canvas.Translate(CellLeft(crack.Cell.Column) + CellSize * 0.5f, FloorY(crack.Cell.Row) - CellSize * 0.5f);
                _parts.Put(layer, layer.ByValue(crack.Progress), DrawBand.Cracks, Color.white);
                Canvas.Restore();
            }
        }
    }
}
