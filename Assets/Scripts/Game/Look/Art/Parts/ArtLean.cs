using System;
using UnityEngine;

namespace Scrapwalker.Game.Look.Art.Parts
{
    [Serializable]
    public sealed class ArtLean : ArtPart
    {
        [SerializeField] private Vector2 _pivot;
        [SerializeField] private float _lean;
        [SerializeField] private float _recoilAngle;
        [SerializeField] private float _recoilFrames;
        [SerializeField] private ArtBump _bump;

        public ArtLean(Vector2 pivot, float lean, float recoilAngle, float recoilFrames, ArtBump bump)
        {
            _pivot = pivot;
            _lean = lean;
            _recoilAngle = recoilAngle;
            _recoilFrames = recoilFrames;
            _bump = bump;
        }

        public override bool Place(IArtStage stage, ArtState state, ArtLayer layer)
        {
            float tilt = state.IsWalking ? _lean : 0f;
            if (_recoilFrames > 0f)
            {
                tilt += _recoilAngle * state.RecoilLeft(layer, _recoilFrames);
            }

            stage.Translate(0f, state.IsWalking ? -_bump.At(state.Clock) : 0f);
            stage.RotateAbout(_pivot, tilt);
            return true;
        }
    }
}
