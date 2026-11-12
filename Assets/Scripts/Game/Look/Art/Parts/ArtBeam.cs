using System;
using UnityEngine;

namespace Scrapwalker.Game.Look.Art.Parts
{
    [Serializable]
    public sealed class ArtBeam : ArtPart
    {
        [SerializeField] private Vector2 _from;
        [SerializeField] private Vector2 _to;

        public ArtBeam(Vector2 from, Vector2 to)
        {
            _from = from;
            _to = to;
        }

        public override bool Place(IArtStage stage, ArtState state, ArtLayer layer)
        {
            if (!state.TryGetCurrentCell(out Vector2 cell))
            {
                return false;
            }

            Vector2 target = stage.FromCanvas(cell);
            Vector2 d = target - _from;
            float length = (_to - _from).magnitude;
            stage.Translate(_from.x, _from.y);
            stage.Rotate(Pen.Direction(_from, target));
            stage.Scale(length > 0f ? d.magnitude / length : 1f, 1f);
            stage.Translate(state.Origin.x, state.Origin.y);
            return true;
        }
    }
}
