using System;
using UnityEngine;

namespace Scrapwalker.Game.Look.Art.Parts
{
    [Serializable]
    public sealed class ArtSmoke : ArtPart
    {
        [SerializeField] private Vector2 _at;

        public ArtSmoke(Vector2 at)
        {
            _at = at;
        }

        public override bool DrawsItself => true;

        public override void Draw(IArtStage stage, ArtState state, ArtLayer layer)
        {
            for (int i = 0; i < 2; i++)
            {
                float y = _at.y - Mathf.Repeat(state.Clock * 0.6f + i * 20f, _at.y - 4f);
                stage.FillRect(_at.x, y, 4f, 6f, Palette.Smoke);
            }
        }
    }
}
